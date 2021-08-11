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
using System.Reflection;

namespace OTAPI.Client.Launcher.Actions
{
    static class OTAPI
    {
        public static void Launch(string[] args)
        {
            Console.WriteLine("[OTAPI.Client] Starting!");

            // reset the runtime paths
            ModFramework.Plugins.PluginLoader.Clear();
            CSharpLoader.GlobalRootDirectory = Path.Combine("csharp");
            CSharpLoader.GlobalAssemblies.Clear();

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            var Terraria = LoadAndCacheAssembly(Path.Combine(Environment.CurrentDirectory, "OTAPI.exe"));

            Terraria.EntryPoint!.Invoke(null, new object[] { args });
        }

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

        private static Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
        {
            var asmName = new AssemblyName(args.Name);

            if (asmName.Name is null) return null;

            Console.WriteLine("[OTAPI Host] Resolving assembly: " + args.Name);
            if (args.Name.StartsWith("ReLogic")) // this occurs as the assembly name is encoded in the xna content files
                return LoadAndCacheAssembly(Path.Combine(Environment.CurrentDirectory, "OTAPI.exe"));

            else if (args.Name.StartsWith("ImGuiNET"))
                return LoadAndCacheAssembly(Path.Combine(Environment.CurrentDirectory, "ImGui.NET.dll"));

            return null;
        }
    }
}
