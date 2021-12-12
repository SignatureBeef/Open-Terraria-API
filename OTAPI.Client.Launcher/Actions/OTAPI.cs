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
        static Assembly Terraria;

        public static void Launch(string[] args)
        {
            Console.WriteLine("[OTAPI.Client] Starting!");

            NativeLibrary.SetDllImportResolver(typeof(Microsoft.Xna.Framework.Game).Assembly, ResolveNativeDep);

            var steam = Path.Combine(Environment.CurrentDirectory, "client", "Steamworks.NET.dll");
            if (File.Exists(steam))
                NativeLibrary.SetDllImportResolver(System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(steam), ResolveNativeDep);

            GC.Collect();

            // reset the runtime paths
            ModFramework.Plugins.PluginLoader.Clear();
            CSharpLoader.GlobalRootDirectory = Path.Combine("csharp");
            CSharpLoader.GlobalAssemblies.Clear();

            CSharpLoader.OnCompilationContext += CSharpLoader_OnCompilationContext;

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            Terraria = LoadAndCacheAssembly(Path.Combine(Environment.CurrentDirectory, "client", "OTAPI.exe"));

            Terraria.EntryPoint!.Invoke(null, new object[] { args });
        }

        private static void CSharpLoader_OnCompilationContext(object? sender, CSharpLoader.CompilationContextArgs e)
        {
            e.Context.Compilation = e.Context.Compilation.WithReferences(
                e.Context.Compilation.References.Where(r =>
                    r.Display.IndexOf("OTAPI.Patcher") == -1
                    && r.Display.IndexOf("System.IO.Compression.Native") == -1
                )
            );
        }

        static Dictionary<string, Assembly?> _assemblyCache = new Dictionary<string, Assembly?>();
        static Dictionary<string, IntPtr> _nativeCache = new Dictionary<string, IntPtr>();

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

        static string ResolveFile(string path)
        {
            var filename = Path.GetFileName(path);
            path = Path.Combine(Environment.CurrentDirectory, filename);

            if (!File.Exists(path))
                path = Path.Combine(Environment.CurrentDirectory, "bin", filename);

            if (!File.Exists(path))
                path = Path.Combine(AppContext.BaseDirectory, filename);

            if (!File.Exists(path))
                path = Path.Combine(Environment.CurrentDirectory, "client", filename);

            return path;
        }

        private static Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
        {
            var asmName = new AssemblyName(args.Name);

            if (asmName.Name is null) return null;

            if (_assemblyCache.TryGetValue(asmName.Name, out Assembly? cached))
                return cached;

            Console.WriteLine("[OTAPI Host] Resolving assembly: " + args.Name);
            if (args.Name.StartsWith("ReLogic") // this occurs as the assembly name is encoded in the xna content files
                || asmName.Name.StartsWith("Terraria")
                || (asmName.Name.StartsWith("OTAPI") && !asmName.Name.StartsWith("OTAPI.Runtime"))
            )
                return Terraria;

            else if (args.Name.StartsWith("OTAPI.Runtime"))
                return LoadAndCacheAssembly(ResolveFile("OTAPI.Runtime.dll"));

            else if (args.Name.StartsWith("ImGuiNET"))
                return LoadAndCacheAssembly(ResolveFile("ImGui.NET.dll"));

            else if (args.Name.StartsWith("Steamworks.NET"))
                return LoadAndCacheAssembly(ResolveFile("Steamworks.NET.dll"));

            else
            {
                var root = typeof(Program).Assembly;
                string resourceName = asmName.Name + ".dll";

                if (File.Exists(resourceName))
                    return LoadAndCacheAssembly(ResolveFile(resourceName));

                var text = Array.Find(root.GetManifestResourceNames(), (element) => element.EndsWith(resourceName));
                if (text != null)
                {
                    Console.WriteLine("Loading from resources " + resourceName);
                    using var stream = root.GetManifestResourceStream(text);
                    if (stream is null) return null;
                    byte[] array = new byte[stream.Length];
                    stream.Read(array, 0, array.Length);
                    stream.Seek(0, SeekOrigin.Begin);

                    // if (!File.Exists(resourceName))
                    //     File.WriteAllBytes(resourceName, array);

                    try
                    {
                        var asm = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromStream(new MemoryStream(array));
                        CacheAssembly(asm);
                        return asm;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }

            // nothing found, cache to save further lookups
            _assemblyCache.Add(asmName.Name, null);

            return null;
        }

        static IntPtr ResolveNativeDep(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (_nativeCache.TryGetValue(libraryName, out IntPtr cached))
                return cached;

            Console.WriteLine("Looking for " + libraryName);

            IEnumerable<string> matches = Enumerable.Empty<string>();

            foreach (var basePath in new[] {
                Environment.CurrentDirectory,
                AppContext.BaseDirectory,
                Path.Combine(Environment.CurrentDirectory, "client")
            })
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    var osx = Path.Combine(basePath, "osx");
                    if(Directory.Exists(osx))
                        matches = matches.Union(Directory.GetFiles(osx, "*" + libraryName + "*"));
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    var lib64 = Path.Combine(basePath, "lib64");
                    if(Directory.Exists(lib64))
                        matches = matches.Union(Directory.GetFiles(lib64, "*" + libraryName + "*"));
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var x64 = Path.Combine(basePath, "x64");
                    if(Directory.Exists(x64))
                        matches = matches.Union(Directory.GetFiles(x64, "*" + libraryName + "*"));
                }

                if (matches.Count() == 0)
                    matches = Directory.GetFiles(basePath, "*" + libraryName + "*");
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
    }
}
