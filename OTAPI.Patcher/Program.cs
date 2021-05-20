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
using System.Linq;
using System.Reflection;
using System.Text;

namespace OTAPI.Patcher
{
    static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Open Terraria API v{GetVersion()}");

            var pathIn = "TerrariaServer.dll"; // exists when built, as its a depedency in OTAPI.Patcher

            Console.WriteLine("[OTAPI] Extracting embedded binaries for assembly resolution...");
            var extractor = new ResourceExtractor();
            var embeddedResourcesDir = extractor.Extract(pathIn);

            // load modfw plugins. this will load ModFramework.Modules and in turn top level c# scripts
            PluginLoader.TryLoad();

            Directory.CreateDirectory("outputs");

            var assembly_output = Path.Combine("outputs", "OTAPI.dll");
            var runtime_output = Path.Combine("outputs", "OTAPI.Runtime.dll");

            //var assembly_output = "OTAPI.dll";
            //var runtime_output = "OTAPI.Runtime.dll";

            using var mm = new ModFwModder()
            {
                InputPath = pathIn,
                OutputPath = assembly_output,
                MissingDependencyThrow = false,
                //LogVerboseEnabled = true,
                // PublicEverything = true, // this is done in setup

                GACPaths = new string[] { } // avoid MonoMod looking up the GAC, which causes an exception on .netcore
            };
            (mm.AssemblyResolver as DefaultAssemblyResolver)!.AddSearchDirectory(embeddedResourcesDir);
            mm.Read();

            //// merge in ModFramework
            //{
            //    mm.OnReadMod += (m, module) =>
            //    {
            //        if (module.Assembly.Name.Name.StartsWith("ModFramework"))
            //            mm.RelinkAssembly(module);
            //    };
            //    mm.ReadMod(Path.Combine(System.Environment.CurrentDirectory, "ModFramework.dll"));
            //}

            mm.MapDependencies();

            mm.AutoPatch();

#if tModLoaderServer_V1_3
                mm.WriterParameters.SymbolWriterProvider = null;
                mm.WriterParameters.WriteSymbols = false;
#endif

            {
                var sac = mm.Module.ImportReference(typeof(AssemblyInformationalVersionAttribute).GetConstructors()[0]);
                var sa = new CustomAttribute(sac);
                sa.ConstructorArguments.Add(new CustomAttributeArgument(mm.Module.TypeSystem.String, GetVersion()));
                mm.Module.Assembly.CustomAttributes.Add(sa);
            }

            mm.Write();

            mm.Log("[OTAPI] Generating OTAPI.Runtime.dll");
            var gen = new MonoMod.RuntimeDetour.HookGen.HookGenerator(mm, "OTAPI.Runtime.dll");
            using (ModuleDefinition mOut = gen.OutputModule)
            {
                gen.Generate();

                mOut.Write(runtime_output);
            }

            ModFramework.Relinker.CoreLibRelinker.PostProcessCoreLib(assembly_output, runtime_output);

            mm.Log("[OTAPI] Building NuGet package...");
            BuildNuGetPackage();

            mm.Log("[OTAPI] Done.");
        }

        static string GetVersion()
        {
            return typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }

        static void BuildNuGetPackage()
        {
            const string packageFile = "OTAPI.nupkg";

            var nuspec_xml = File.ReadAllText("../../../../OTAPI.nuspec");
            nuspec_xml = nuspec_xml.Replace("[INJECT_VERSION]", GetVersion());

            using (var nuspec = new MemoryStream(Encoding.UTF8.GetBytes(nuspec_xml)))
            {
                var manifest = NuGet.Packaging.Manifest.ReadFrom(nuspec, validateSchema: true);
                var packageBuilder = new NuGet.Packaging.PackageBuilder();
                packageBuilder.Populate(manifest.Metadata);

                packageBuilder.AddFiles("../../../../", "COPYING.txt", "COPYING.txt");
                packageBuilder.AddFiles(Environment.CurrentDirectory, "OTAPI.dll", "lib\\net5.0");
                packageBuilder.AddFiles(Environment.CurrentDirectory, "OTAPI.Runtime.dll", "lib\\net5.0");
                packageBuilder.AddFiles(Environment.CurrentDirectory, "OTAPI.dll", "lib\\netstandard2.0");
                packageBuilder.AddFiles(Environment.CurrentDirectory, "OTAPI.Runtime.dll", "lib\\netstandard2.0");

                if (File.Exists(packageFile))
                    File.Delete(packageFile);

                using (var srm = File.OpenWrite(packageFile))
                    packageBuilder.Save(srm);
            }
        }
    }
}
