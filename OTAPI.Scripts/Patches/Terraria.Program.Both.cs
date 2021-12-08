/*
Copyright (C) 2020 DeathCradle

This file is part of Open Terraria API v3 (OTAPI)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/
#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
#pragma warning disable CS0436 // Type conflicts with imported type

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using ModFramework;
using ModFramework.Plugins;

/// <summary>
/// @doc Fixes platform issues in Terraria.Program.DisplayException
/// </summary>
/// <summary>
/// @doc Initiates the client module system
/// </summary>
namespace Terraria
{
    partial class patch_Program
    {
#if !Terraria // the client has a customer MessageBox.Show implementation on avalonia
        // no WinForms access so just write to console
        public static extern void orig_DisplayException(Exception e);
        public static void DisplayException(Exception e)
        {
            Console.WriteLine(e.ToString());
        }
#endif

        /// <summary>
        /// Triggers when mods should start attaching events. At this point assembly resolution should be ready on all platforms.
        /// </summary>
        public static event EventHandler OnLaunched;

        static string CSV(params string[] args) => String.Join(",", args.Where(x => !String.IsNullOrWhiteSpace(x)));

        private static void Configure()
        {
            // https://github.com/FNA-XNA/FNA/wiki/4:-FNA-and-Windows-API#64-bit-support
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                var path = Path.Combine(
                    Environment.CurrentDirectory,
                    Environment.Is64BitProcess ? "x64" : "x86"
                );
                Directory.CreateDirectory(path);

                try
                {
                    SetDefaultDllDirectories(LOAD_LIBRARY_SEARCH_DEFAULT_DIRS);
                    AddDllDirectory(path);
                }
                catch
                {
                    // Pre-Windows 7, KB2533623 
                    SetDllDirectory(path);
                }
            }

            // https://github.com/FNA-XNA/FNA/wiki/7:-FNA-Environment-Variables#fna_graphics_enable_highdpi
            // NOTE: from documentation: 
            //       Lastly, when packaging for macOS, be sure this is in your app bundle's Info.plist:
            //           <key>NSHighResolutionCapable</key>
            //           <string>True</string>
            Environment.SetEnvironmentVariable("FNA_GRAPHICS_ENABLE_HIGHDPI", "1");
        }

        /// <summary>
        /// Root entry point for OTAPI. Host games can use OTAPI.Runtime.dll to override the 
        /// </summary>
        public static void LaunchOTAPI()
        {
            Configure();

            Console.WriteLine($"[OTAPI] Starting up ({CSV(OTAPI.Common.Target, OTAPI.Common.Version, OTAPI.Common.GitHubCommit)}).");

            // load modfw plugins
            PluginLoader.TryLoad();
            Modifier.Apply(ModType.Runtime, optionalParams: new[] { Assembly.GetExecutingAssembly() }); // set Assembly type to Terraria/OTAPI for runtime plugins to resolve easier when they dont have a direct ref

#if Terraria // client
            Main.versionNumber += " OTAPI";
            Main.versionNumber2 += " OTAPI";
#endif

            OnLaunched?.Invoke(null, EventArgs.Empty);
        }

        public static void ShutdownOTAPI()
        {
            // give modfw mods the chance to safely shutdown. e.g. currently the csharp module can run scripts, and some of those scripts (currently) use native Cef.
            Modifier.Apply(ModType.Shutdown, optionalParams: new[] { Assembly.GetExecutingAssembly() });
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetDefaultDllDirectories(int directoryFlags);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern void AddDllDirectory(string lpPathName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetDllDirectory(string lpPathName);

        const int LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000;

#if tModLoaderServer
        public static extern void orig_LaunchGame_();
        public static void LaunchGame_()
        {
            LaunchOTAPI();
            orig_LaunchGame_();
            ShutdownOTAPI();
        }
#else // server + client
        public static extern void orig_LaunchGame(string[] args, bool monoArgs = false);
        public static void LaunchGame(string[] args, bool monoArgs = false)
        {
            LaunchOTAPI();
            orig_LaunchGame(args, monoArgs);
            ShutdownOTAPI();
        }
#endif
    }
}

// just a stub for rosyln - this is defined in another patch
namespace OTAPI
{
    [MonoMod.MonoModIgnore]
    public static partial class Common
    {
        public static readonly string Target;
        public static readonly string Version;
        public static readonly string GitHubCommit;
    }
}
