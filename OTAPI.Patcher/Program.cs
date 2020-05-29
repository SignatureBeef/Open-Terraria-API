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
using Mono.Cecil;
using MonoMod;
using OTAPI.Common;
using System;
using System.IO;
using System.Linq;

namespace OTAPI.Patcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var pathIn = Remote.DownloadServer();

            Console.WriteLine($"[OTAPI] Extracting embedded binaries and packing into one binary...");
            var extractor = new ResourceExtractor();
            var embeddedResourcesDir = extractor.Extract(pathIn);

            // var repacker = new ILRepacking.ILRepack(new ILRepacking.RepackOptions()
            // {
            //     InputAssemblies = new[] { pathIn, Directory.GetFiles(embeddedResourcesDir).Single(x => Path.GetFileName(x).Equals("ReLogic.dll", StringComparison.CurrentCultureIgnoreCase)) }.ToArray(),
            //     SearchDirectories = new[] { Path.GetDirectoryName(pathIn), embeddedResourcesDir },

            //     OutputFile = "TerrariaServer.dll"
            // });
            // repacker.Repack();

            using (var mm = new MonoModder()
            {
                InputPath = pathIn, //"TerrariaServer.dll",
                OutputPath = "OTAPI.dll",
                MissingDependencyThrow = false,
                //LogVerboseEnabled = true,
                PublicEverything = true, // we want all of terraria exposed
                
                GACPaths = new string[] { } // avoid MonoMod looking up the GAC, which causes an exception on .netcore
            })
            {
                //mm.AssemblyResolver = new ResourceAssemblyResolver(mm);
                (mm.AssemblyResolver as DefaultAssemblyResolver).AddSearchDirectory(embeddedResourcesDir);
                mm.Read();

                foreach (var path in new[] {
                    Path.Combine(System.Environment.CurrentDirectory, "TerrariaServer.OTAPI.Shims.mm.dll"),
                    Path.Combine(System.Environment.CurrentDirectory, "TerrariaServer.OTAPI.mm.dll"),
                    Directory.GetFiles(embeddedResourcesDir).Single(x => Path.GetFileName(x).Equals("ReLogic.dll", StringComparison.CurrentCultureIgnoreCase)),
                    //Directory.GetParent(pathIn).FullName,
                    //"../../../../OTAPI.Mods/bin/Debug/netstandard2.0"
                })
                {
                    mm.Log($"[MonoMod] Reading mod or directory: {path}");
                    mm.ReadMod(path);
                }

                mm.MapDependencies();
                mm.AutoPatch();

                OTAPI.Mods.Modifier.Apply(OTAPI.Mods.ModificationType.Patchtime, mm);

                mm.Write();

                mm.Log("[MonoMod] Done.");
            }
        }
    }
}
