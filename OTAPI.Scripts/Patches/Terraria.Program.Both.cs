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
            // we are now on net5 and terraria never knows to do this, so using the new frameworks we need to load
            // assemblies from the EmbeddedResources of the Terraria exe upon request.
            System.Runtime.Loader.AssemblyLoadContext.Default.Resolving += ResolveDependency;

            NativeLibrary.SetDllImportResolver(typeof(Microsoft.Xna.Framework.Game).Assembly, ResolveNativeDep);

            var steam = Path.Combine(Environment.CurrentDirectory, "Steamworks.NET.dll");
            if (File.Exists(steam))
                NativeLibrary.SetDllImportResolver(System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(steam), ResolveNativeDep);

            // https://github.com/FNA-XNA/FNA/wiki/4:-FNA-and-Windows-API#64-bit-support
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                var path = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    Environment.Is64BitProcess ? "x64" : "x86"
                );
                Directory.CreateDirectory(path);
                SetDllDirectory(path);
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

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetDllDirectory(string lpPathName);

        static IntPtr ResolveNativeDep(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (_nativeCache.TryGetValue(libraryName, out IntPtr cached))
                return cached;

            Console.WriteLine("Looking for " + libraryName);

            IEnumerable<string> matches = Enumerable.Empty<string>();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var osx = Path.Combine(Environment.CurrentDirectory, "osx");
                matches = Directory.GetFiles(osx, "*" + libraryName + "*");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var lib64 = Path.Combine(Environment.CurrentDirectory, "lib64");
                matches = Directory.GetFiles(lib64, "*" + libraryName + "*");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var x64 = Path.Combine(Environment.CurrentDirectory, "x64");
                matches = Directory.GetFiles(x64, "*" + libraryName + "*");
            }

            if (matches.Count() == 0)
            {
                matches = Directory.GetFiles(Environment.CurrentDirectory, "*" + libraryName + "*");
            }

            var handle = IntPtr.Zero;

            if (matches.Count() == 1)
            {
                var match = matches.Single();
                handle = NativeLibrary.Load(match);
            }

            // cache either way. if zero, no point calling IO if we've checked this assembly before.
            _nativeCache.Add(libraryName, handle);

            return handle;
        }

        static Dictionary<string, IntPtr> _nativeCache = new Dictionary<string, IntPtr>();
        static Dictionary<string, Assembly> _assemblyCache = new Dictionary<string, Assembly>();

        static void CacheAssembly(Assembly assembly)
        {
            var name = assembly.GetName().Name;
            if (name is not null && !_assemblyCache.ContainsKey(name))
            {
                _assemblyCache.Add(name, assembly);
            }
        }

        static Assembly LoadAndCacheAssembly(string filePath)
        {
            var abs = Path.Combine(Environment.CurrentDirectory, filePath);
            //var result = Assembly.LoadFile(abs);
            var result = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(filePath);
            CacheAssembly(result);

            return result;
        }

        private static Assembly ResolveDependency(System.Runtime.Loader.AssemblyLoadContext ctx, AssemblyName asmName)
        {
            if (asmName.Name is null) return null;

            if (_assemblyCache.TryGetValue(asmName.Name, out Assembly? cached))
                return cached;

            Console.WriteLine("[OTAPI] Resolving assembly: " + asmName.Name);
            if (asmName.Name.StartsWith("Terraria") || (asmName.Name.StartsWith("OTAPI") && !asmName.Name.StartsWith("OTAPI.Runtime")))
            {
                if (!_assemblyCache.ContainsKey(asmName.Name))
                    _assemblyCache.Add(asmName.Name, typeof(Program).Assembly);
                return typeof(Program).Assembly;
            }
            //else if (asmName.Name.StartsWith("ImGuiNET")) todo: investigate why this doesnt work here - only works in the host/launcher
            //{
            //    return LoadAndCacheAssembly(Path.Combine(Environment.CurrentDirectory, "ImGui.NET.dll"));
            //}
            else
            {
                var root = typeof(Program).Assembly;
                string resourceName = asmName.Name + ".dll";

                if (File.Exists(resourceName))
                    return LoadAndCacheAssembly(Path.Combine(Environment.CurrentDirectory, resourceName));

                var text = Array.Find(root.GetManifestResourceNames(), (element) => element.EndsWith(resourceName));
                if (text != null)
                {
                    Console.WriteLine("Loading from resources " + resourceName);
                    using var stream = root.GetManifestResourceStream(text);
                    if (stream is null) return null;
                    byte[] array = new byte[stream.Length];
                    stream.Read(array, 0, array.Length);
                    stream.Seek(0, SeekOrigin.Begin);

                    if (!File.Exists(resourceName))
                        File.WriteAllBytes(resourceName, array);

                    try
                    {
                        var asm = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromStream(new MemoryStream(array));
                        return asm;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

            }
            return null;
        }

#if tModLoaderServer
        public static extern void orig_LaunchGame_();
        public static void LaunchGame_()
        {
            LaunchOTAPI();
            orig_LaunchGame_();
        }
#else // server + client
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
