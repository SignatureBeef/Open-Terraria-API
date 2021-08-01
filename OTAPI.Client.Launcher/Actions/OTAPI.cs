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
using Microsoft.CodeAnalysis;
using ModFramework.Modules.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace OTAPI.Client.Launcher.Actions
{
    static class OTAPI
    {
        static Assembly? Terraria;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetDllDirectory(string lpPathName);

        public static void Launch(string[] args)
        {
            Console.WriteLine("[OTAPI.Client] Starting!");

            // reset the runtime paths
            ModFramework.Plugins.PluginLoader.Clear();
            CSharpLoader.GlobalRootDirectory = Path.Combine("csharp", "plugins");
            CSharpLoader.GlobalAssemblies.Clear();

            // https://github.com/FNA-XNA/FNA/wiki/4:-FNA-and-Windows-API#64-bit-support
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetDllDirectory(Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    Environment.Is64BitProcess ? "x64" : "x86"
                ));
            }

            // https://github.com/FNA-XNA/FNA/wiki/7:-FNA-Environment-Variables#fna_graphics_enable_highdpi
            // NOTE: from documentation: 
            //       Lastly, when packaging for macOS, be sure this is in your app bundle's Info.plist:
            //           <key>NSHighResolutionCapable</key>
            //           <string>True</string>
            Environment.SetEnvironmentVariable("FNA_GRAPHICS_ENABLE_HIGHDPI", "1");

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            AppDomain.CurrentDomain.TypeResolve += CurrentDomain_TypeResolve;

            var asm_path = Path.Combine(Environment.CurrentDirectory, "OTAPI.exe");
            Terraria = LoadAndCacheAssembly(asm_path);
            var steam = LoadAndCacheAssembly("Steamworks.NET.dll");

            NativeLibrary.SetDllImportResolver(typeof(Microsoft.Xna.Framework.Game).Assembly, ResolveNativeDep);
            NativeLibrary.SetDllImportResolver(steam, ResolveNativeDep);

            CSharpLoader.OnCompilationContext += ResolveSystemRefs;

            Terraria.EntryPoint!.Invoke(null, new object[] { args });
        }

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

        static void ResolveSystemRefs(object? instance, ModFramework.Modules.CSharp.CSharpLoader.CompilationContextArgs args)
        {
            if (args.Context is null) return;
            if (args.Context.Compilation is null) return;

            var asms = System.AppDomain.CurrentDomain.GetAssemblies();
            if (args.CoreLibAssemblies is not null && args.CoreLibAssemblies.Count() == 0)
            {
                foreach (var asm in asms.Where(x => !x.IsDynamic))
                {
                    if (!string.IsNullOrWhiteSpace(asm.Location) && System.IO.File.Exists(asm.Location))
                        args.Context.Compilation = args.Context.Compilation.AddReferences(MetadataReference.CreateFromFile(asm.Location));
                }

                foreach (var file in Directory.GetFiles(Environment.CurrentDirectory, "Syste*.dll"))
                    args.Context.Compilation = args.Context.Compilation.AddReferences(MetadataReference.CreateFromFile(file));

                args.Context.Compilation = args.Context.Compilation.AddReferences(MetadataReference.CreateFromFile("netstandard.dll"));
                args.Context.Compilation = args.Context.Compilation.AddReferences(MetadataReference.CreateFromFile("mscorlib.dll"));
            }
        }

        private static Assembly? CurrentDomain_TypeResolve(object? sender, ResolveEventArgs args)
        {
            Console.WriteLine("Looking for type: " + args.Name);
            return null;
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
            var result = Assembly.LoadFile(abs);
            CacheAssembly(result);

            return result;
        }

        private static Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
        {
            var asmName = new AssemblyName(args.Name);

            if (asmName.Name is null) return null;

            if (_assemblyCache.TryGetValue(asmName.Name, out Assembly? cached))
                return cached;

            Console.WriteLine("[OTAPI] Resolving assembly: " + args.Name);
            if (args.Name.StartsWith("Terraria") || (args.Name.StartsWith("OTAPI") && !args.Name.StartsWith("OTAPI.Runtime"))
                || args.Name.StartsWith("ReLogic") // shouldnt really get here unless bad IL
            )
            {
                if (Terraria is null) throw new Exception("Failed to resolve terraria assembly");
                if (!_assemblyCache.ContainsKey(asmName.Name))
                    _assemblyCache.Add(asmName.Name, Terraria);
                return Terraria;
            }
            else if (args.Name.StartsWith("ImGuiNET"))
            {
                return LoadAndCacheAssembly(Path.Combine(Environment.CurrentDirectory, "ImGui.NET.dll"));
            }
            else
            {
                if (Terraria is null) throw new Exception("Failed to resolve terraria assembly");
                var root = Terraria;
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
                        var asm = System.Reflection.Assembly.Load(array);
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
    }
}
