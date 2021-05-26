// Copyright (C) 2020-2021 DeathCradle
//
// This file is part of Open Terraria API v3 (OTAPI)
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// using System;
using System;
using System.IO;
using System.Reflection;
using ModFramework;
using ModFramework.Plugins;
using Mono.Cecil;

namespace OTAPI.Patcher.Targets
{
    public class VanillaClientPatchTarget : IPatchTarget
    {
        public string DisplayText { get; } = "Vanilla Client (Moddable)";

        HookResult CanLoadFile(string filepath)
        {
            // only load "server" or "both" variants
            var filename = Path.GetFileNameWithoutExtension(filepath);
            var result = filename.EndsWith(".Server", StringComparison.CurrentCultureIgnoreCase)
                ? HookResult.Cancel : HookResult.Continue;

            return result;
        }

        public void Patch()
        {
            Console.WriteLine($"Open Terraria API v{GetVersion()} [lightweight]");

            var freshAssembly = "../../../../OTAPI.Setup/bin/Debug/net5.0/Terraria.exe";
            var localPath = "Terraria.exe";

            if (File.Exists(localPath)) File.Delete(localPath);
            File.Copy(freshAssembly, localPath);

            Console.WriteLine("[OTAPI] Extracting embedded binaries for assembly resolution...");
            var extractor = new ResourceExtractor();
            var embeddedResourcesDir = extractor.Extract(localPath);

            var installPath = DetermineClientInstallPath();
            var resources = Path.Combine(installPath, "Resources");
            var assembly_output = Path.Combine(installPath, "Resources/Terraria.patched.exe");
            var runtime_output = Path.Combine(installPath, "Resources/Terraria.Runtime.dll");
            var mfw_output = Path.Combine(installPath, "Resources/ModFramework.dll");

            // load modfw plugins. this will load ModFramework.Modules and in turn top level c# scripts
            PluginLoader.AssemblyFound += CanLoadFile;
            ModFramework.Modules.CSharpLoader.AssemblyFound += CanLoadFile;
            ModFramework.Modules.CSharpLoader.GlobalAssemblies.Add(localPath);
            ModFramework.Modules.CSharpLoader.GlobalAssemblies.Add(Path.Combine(resources, "FNA.dll"));
            ModFramework.Modules.CSharpLoader.GlobalAssemblies.Add(Path.Combine(Path.GetDirectoryName(typeof(Object).Assembly.Location), "mscorlib.dll"));
            PluginLoader.TryLoad();

            using var mm = new ModFwModder()
            {
                InputPath = localPath,
                OutputPath = assembly_output,
                MissingDependencyThrow = false,
                //LogVerboseEnabled = true,
                // PublicEverything = true, // this is done in setup

                GACPaths = new string[] { } // avoid MonoMod looking up the GAC, which causes an exception on .netcore
            };
            (mm.AssemblyResolver as DefaultAssemblyResolver)!.AddSearchDirectory(embeddedResourcesDir);
            (mm.AssemblyResolver as DefaultAssemblyResolver)!.AddSearchDirectory(Path.Combine(installPath, "Resources"));
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

            //{
            //    var sac = mm.Module.ImportReference(typeof(AssemblyInformationalVersionAttribute).GetConstructors()[0]);
            //    var sa = new CustomAttribute(sac);
            //    sa.ConstructorArguments.Add(new CustomAttributeArgument(mm.Module.TypeSystem.String, GetVersion()));
            //    mm.Module.Assembly.CustomAttributes.Add(sa);
            //}

            foreach (var asmref in mm.Module.AssemblyReferences.ToArray())
            {
                if (asmref.Name.Contains("System.Private.CoreLib"))
                {
                    mm.Module.AssemblyReferences.Remove(asmref);
                }
            }

            mm.Write();

            mm.Log("[OTAPI] Generating OTAPI.Runtime.dll");
            var gen = new MonoMod.RuntimeDetour.HookGen.HookGenerator(mm, "OTAPI.Runtime.dll");
            using (ModuleDefinition mOut = gen.OutputModule)
            {
                gen.Generate();

                mOut.Write(runtime_output);
            }

            if (File.Exists(mfw_output)) File.Delete(mfw_output);
            File.Copy("ModFramework.dll", mfw_output);

            mm.Log("[OTAPI] Done.");
        }

        static string GetVersion()
        {
            return typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }

        string[] SearchPaths { get; } = new[]
        {
            "/Users/[USER_NAME]/Library/Application Support/Steam/steamapps/common/Terraria/Terraria.app/Contents/",
        };

        bool IsValidInstallPath(string folder)
        {
            bool valid = Directory.Exists(folder);

            var macOS = Path.Combine(folder, "MacOS");
            var resources = Path.Combine(folder, "Resources");

            var startScript = Path.Combine(macOS, "Terraria");
            var startBin = Path.Combine(macOS, "Terraria.bin.osx");
            var assembly = Path.Combine(resources, "Terraria.exe");
            var fna = Path.Combine(resources, "FNA.dll");

            valid &= Directory.Exists(macOS);
            valid &= Directory.Exists(resources);

            valid &= File.Exists(startScript);
            valid &= File.Exists(startBin);
            valid &= File.Exists(assembly);
            valid &= File.Exists(fna);

            return valid;
        }

        string DetermineClientInstallPath()
        {
            foreach (var path in SearchPaths)
            {
                var formatted = path.Replace("[USER_NAME]", Environment.UserName);
                if (IsValidInstallPath(formatted))
                    return formatted;
            }
            // /Users/luke/Library/Application Support/Steam/steamapps/common/Terraria/Terraria.app/Contents/
            //Environment.UserName

            int count = 5;
            do
            {
                Console.Write("What is the Terraria client install FOLDER?: ");
                var path = Console.ReadLine();

                if (!String.IsNullOrWhiteSpace(path))
                {
                    if (IsValidInstallPath(path))
                        return path;
                }

                Console.WriteLine("Invalid folder or wrong install folder.");
            }
            while (count-- > 0);

            throw new DirectoryNotFoundException();
        }
    }
}
