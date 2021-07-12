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

using System;
using System.Linq;
using System.Reflection;
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
        // no WinForms access so just write to console
        public static extern void orig_DisplayException(Exception e);
        public static void DisplayException(Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        /// <summary>
        /// Triggers when mods should start attaching events. At this point assembly resolution should be ready on all platforms.
        /// </summary>
        public static event EventHandler OnLaunched;

        /// <summary>
        /// Allows a host application to determine if ModFramework is invoked.
        /// </summary>
        public static bool EnableModFramework { get; set; } = true;

        static string CSV(params string[] args) => String.Join(",", args.Where(x => !String.IsNullOrWhiteSpace(x)));

        /// <summary>
        /// Root entry point for OTAPI. Host games can use OTAPI.Runtime.dll to override the 
        /// </summary>
        public static void LaunchOTAPI()
        {
            // we are now on net5 and terraria never knows to do this, so using the new frameworks we need to load
            // assemblies from the EmbeddedResources of the Terraria exe upon request.
            System.Runtime.Loader.AssemblyLoadContext.Default.Resolving += ResolveDependency;

            Console.WriteLine($"[OTAPI] Starting up ({CSV(OTAPI.Common.Target, OTAPI.Common.Version, OTAPI.Common.GitHubCommit)}).");

            if (EnableModFramework)
            {
                PluginLoader.TryLoad();
                Modifier.Apply(ModType.Runtime);
            }

#if Terraria // client
            Main.versionNumber += " OTAPI";
            Main.versionNumber2 += " OTAPI";
#endif

            OnLaunched?.Invoke(null, EventArgs.Empty);
        }

        public static Assembly ResolveDependency(System.Runtime.Loader.AssemblyLoadContext ctx, AssemblyName assemblyName)
        {
            Console.WriteLine($"Looking for assembly: {assemblyName.Name}");
            var resourceName = assemblyName.Name + ".dll";
            var src = typeof(Program).Assembly;
            resourceName = Array.Find(src.GetManifestResourceNames(), element => element.EndsWith(resourceName));

            if (!string.IsNullOrWhiteSpace(resourceName))
            {
                Console.WriteLine($"[OTAPI] Resolved ${resourceName}");
                using (var stream = src.GetManifestResourceStream(resourceName))
                    return System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromStream(stream);
            }

            return null;
        }

#if Terraria // client
        public static extern void orig_LaunchGame(string[] args, bool monoArgs = false);
        public static void LaunchGame(string[] args, bool monoArgs = false)
        {
            LaunchOTAPI();
            orig_LaunchGame(args, monoArgs);
        }
#elif tModLoaderServer // tml
        public static extern void orig_LaunchGame_();
        public static void LaunchGame_()
        {
            LaunchOTAPI();
            orig_LaunchGame_();
        }
#else // server
        public static extern void orig_LaunchGame(string[] args, bool monoArgs = false);
        public static void LaunchGame(string[] args, bool monoArgs = false)
        {
            LaunchOTAPI();
            orig_LaunchGame(args, monoArgs);
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