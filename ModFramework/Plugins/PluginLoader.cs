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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ModFramework.Plugins
{
    public delegate bool AssemblyFoundHandler(string filepath);

    public static class PluginLoader
    {
        private static List<Assembly> _assemblies;

        public static IEnumerable<Assembly> Assemblies => _assemblies;

        public static IAssemblyLoader AssemblyLoader { get; set; }

        public static event AssemblyFoundHandler AssemblyFound;

        public static void AddAssembly(Assembly assembly)
        {
            _assemblies.Add(assembly);
        }

        public static void Init()
        {
            if (AssemblyLoader == null)
                AssemblyLoader = new DefaultAssemblyLoader();

            _assemblies = new List<Assembly>();
        }

        public static bool TryLoad()
        {
            if (_assemblies == null)
            {
                Init();

                _assemblies.AddRange(new[] {
                    Assembly.GetExecutingAssembly(),
                    Assembly.GetCallingAssembly(),
                }.Distinct());

                //IEnumerable<string> files = Directory.EnumerateFiles(Environment.CurrentDirectory, "*.mm.dll", SearchOption.AllDirectories);

                IEnumerable<string> files = Enumerable.Empty<string>();
                if (Directory.Exists("modifications"))
                {
                    files = files.Concat(Directory.EnumerateFiles("modifications", "*.dll", SearchOption.AllDirectories));
                }

                foreach (var file in files.Distinct())
                {
                    try
                    {
                        if (AssemblyFound?.Invoke(file) == false)
                            continue; // event was cancelled, they do not wish to use this file. skip to the next.

                        Console.WriteLine($"[ModFw:Startup] Loading {file}");
                        var asm = AssemblyLoader.Load(file);
                        _assemblies.Add(asm);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ModFw:Startup] Load failed {ex}");
                    }
                }

                return true;
            }
            return false;
        }

        public static void Clear()
        {
            _assemblies?.Clear();
            _assemblies = null;
        }
    }
}
