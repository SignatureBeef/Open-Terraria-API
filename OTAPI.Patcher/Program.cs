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
using OTAPI.Common;
using System;
using System.IO;

namespace OTAPI.Patcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var pathIn = Remote.DownloadServer();

            Console.WriteLine($"[OTAPI] Extracting embedded binaries for assembly resolution...");
            var extractor = new ResourceExtractor();
            var embeddedResourcesDir = extractor.Extract(pathIn);

            using (var mm = new OTAPIModder()
            {
                InputPath = "TerrariaServer.dll", // exists when built, as its a depedency in OTAPI.Mods
                OutputPath = "OTAPI.dll",
                MissingDependencyThrow = false,
                //LogVerboseEnabled = true,
                // PublicEverything = true, // we want all of terraria exposed

                GACPaths = new string[] { } // avoid MonoMod looking up the GAC, which causes an exception on .netcore
            })
            {
                (mm.AssemblyResolver as DefaultAssemblyResolver).AddSearchDirectory(embeddedResourcesDir);
                mm.Read();

                foreach (var path in new[] {
                    //Path.Combine(System.Environment.CurrentDirectory, "TerrariaServer.OTAPI.Shims.mm.dll"),
                    Path.Combine(System.Environment.CurrentDirectory, "TerrariaServer.OTAPI.mm.dll"),
                    // Directory.GetFiles(embeddedResourcesDir).Single(x => Path.GetFileName(x).Equals("ReLogic.dll", StringComparison.CurrentCultureIgnoreCase)),
                })
                {
                    mm.Log($"[MonoMod] Reading mod or directory: {path}");
                    mm.ReadMod(path);
                }

                mm.MapDependencies();
                mm.AutoPatch();

                OTAPI.Modifier.Apply(OTAPI.ModType.PostProcess, mm);

                mm.Write();

                mm.Log("[MonoMod] Done.");
            }
        }
    }
}
