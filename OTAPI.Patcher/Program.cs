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
using ModFramework;
using ModFramework.Plugins;
using Mono.Cecil;
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

            PluginLoader.TryLoad();

            using (var mm = new ModFwModder()
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

                Modifier.Apply(ModType.Read, mm);

                foreach (var path in new[] {
                    Path.Combine(System.Environment.CurrentDirectory, "ModFramework.dll"),
                    Path.Combine(System.Environment.CurrentDirectory, "TerrariaServer.OTAPI.mm.dll"),
                })
                {
                    mm.ReadMod(path);
                }

                mm.MapDependencies();

                Modifier.Apply(ModType.PrePatch);

                mm.AutoPatch();

                Modifier.Apply(ModType.PostPatch, mm);

#if tModLoaderServer_V1_3
                mm.WriterParameters.SymbolWriterProvider = null;
                mm.WriterParameters.WriteSymbols = false;
#endif

                mm.Write();

                mm.Log("[OTAPI] Generating OTAPI.Runtime.dll");
                var gen = new MonoMod.RuntimeDetour.HookGen.HookGenerator(mm, Path.GetFileName("OTAPI.Runtime.dll"));
                using (ModuleDefinition mOut = gen.OutputModule)
                {
                    gen.Generate();

                    mOut.Write($"OTAPI.Runtime.dll");
                }

                mm.Log("[OTAPI] Done.");
            }
        }
    }
}
