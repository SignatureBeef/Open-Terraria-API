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
using System.Linq;
using System.Reflection;
using ModFramework;
using ModFramework.Plugins;
using Mono.Cecil;
using OTAPI.Common;

namespace OTAPI.Patcher.Targets
{
    public class OTAPIClientLightweightTarget : IPatchTarget
    {
        public string DisplayText { get; } = "OTAPI Client (lightweight)";

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
            Console.WriteLine($"Open Terraria API v{Common.GetVersion()} [lightweight]");

            var installPath = ClientHelpers.DetermineClientInstallPath();

            var input = Path.Combine(installPath, "Resources/Terraria.exe");

            //var freshAssembly = "../../../../OTAPI.Setup/bin/Debug/net5.0/Terraria.exe";
            var localPath = "Terraria.exe";

            if (File.Exists(localPath)) File.Delete(localPath);
            File.Copy(input, localPath);

            Console.WriteLine("[OTAPI] Extracting embedded binaries for assembly resolution...");
            var extractor = new ResourceExtractor();
            var embeddedResourcesDir = extractor.Extract(localPath);

            using (var public_mm = new ModFwModder()
            {
                InputPath = localPath,
                OutputPath = "OTAPI.dll",
                MissingDependencyThrow = false,
                //LogVerboseEnabled = true,
                PublicEverything = true,

                GACPaths = new string[] { } // avoid MonoMod looking up the GAC, which causes an exception on .netcore
            })
            {
                (public_mm.AssemblyResolver as DefaultAssemblyResolver)!.AddSearchDirectory(embeddedResourcesDir);
                (public_mm.AssemblyResolver as DefaultAssemblyResolver)!.AddSearchDirectory(Path.Combine(installPath, "Resources"));
                public_mm.Read();
                public_mm.MapDependencies();
                public_mm.AutoPatch();
                public_mm.Write();
            }

            //var installPath = ClientHelpers.DetermineClientInstallPath();
            var resources = Path.Combine(installPath, "Resources");
            var assembly_output = Path.Combine(installPath, "Resources/OTAPI.exe");
            //var runtime_output = Path.Combine(installPath, "Resources/Terraria.Runtime.dll");
            //var mfw_output = Path.Combine(installPath, "Resources/ModFramework.dll");

            // load modfw plugins. this will load ModFramework.Modules and in turn top level c# scripts
            PluginLoader.AssemblyFound += CanLoadFile;
            ModFramework.Modules.CSharpLoader.AssemblyFound += CanLoadFile;
            ModFramework.Modules.CSharpLoader.GlobalAssemblies.Add(localPath);
            ModFramework.Modules.CSharpLoader.GlobalAssemblies.Add(Path.Combine(resources, "FNA.dll"));
            ModFramework.Modules.CSharpLoader.GlobalAssemblies.Add(Path.Combine(Path.GetDirectoryName(typeof(Object).Assembly.Location), "mscorlib.dll"));
            PluginLoader.TryLoad();

            using var mm = new ModFwModder()
            {
                InputPath = "OTAPI.dll",
                OutputPath = "OTAPI.exe",
                MissingDependencyThrow = false,
                //LogVerboseEnabled = true,
                //PublicEverything = true,

                GACPaths = new string[] { } // avoid MonoMod looking up the GAC, which causes an exception on .netcore
            };
            (mm.AssemblyResolver as DefaultAssemblyResolver)!.AddSearchDirectory(embeddedResourcesDir);
            (mm.AssemblyResolver as DefaultAssemblyResolver)!.AddSearchDirectory(Path.Combine(installPath, "Resources"));
            mm.Read();

            var inputName = Path.GetFileNameWithoutExtension(input);
            var initialModuleName = mm.Module.Name;

            //// prechange the assembly name to a dll
            //// monomod will also reference this when relinking so it must be correct
            //// in order for shims within this dll to work (relogic)
            //mm.Module.Name = "TerrariaServer.dll";
            //mm.Module.Assembly.Name.Name = "TerrariaServer";

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
                if (asmref.Name.Contains("System.Private.CoreLib") || asmref.Name.Contains("netstandard"))
                {
                    mm.Module.AssemblyReferences.Remove(asmref);
                }
            }

            foreach (var mmt in mm.Module.Types.Where(x => x.Namespace == "MonoMod").ToArray())
            {
                mm.Module.Types.Remove(mmt);
            }

            mm.Write();

            if (File.Exists(assembly_output)) File.Delete(assembly_output);
            File.Copy("OTAPI.exe", assembly_output);

            //mm.Log("[OTAPI] Generating Terraria.Runtime.dll");
            //var gen = new MonoMod.RuntimeDetour.HookGen.HookGenerator(mm, "Terraria.Runtime.dll");
            //using (ModuleDefinition mOut = gen.OutputModule)
            //{
            //    gen.Generate();


            //    foreach (var asmref in mOut.AssemblyReferences.ToArray())
            //    {
            //        if (asmref.Name.Contains("System.Private.CoreLib") || asmref.Name.Contains("netstandard"))
            //        {
            //            mOut.AssemblyReferences.Remove(asmref);
            //        }
            //    }

            //    mOut.Write("Terraria.Runtime.dll");
            //    if (File.Exists(runtime_output)) File.Delete(runtime_output);
            //    File.Copy("Terraria.Runtime.dll", runtime_output);
            //}

            //if (File.Exists(mfw_output)) File.Delete(mfw_output);
            //File.Copy("ModFramework.dll", mfw_output);

            var const_major = $"{inputName}_V{mm.Module.Assembly.Name.Version.Major}_{mm.Module.Assembly.Name.Version.Minor}";
            var const_fullname = $"{inputName}_{mm.Module.Assembly.Name.Version.ToString().Replace(".", "_")}";

            File.WriteAllText("AutoGenerated.target", @$"<!-- DO NOT EDIT THIS FILE! It was auto generated by the setup project  -->
<Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
    <DefineConstants>{inputName};{const_major};{const_fullname}</DefineConstants>
  </PropertyGroup>
</Project>");
            File.WriteAllText("AutoGenerated.cs", @$"#define {inputName}
#define {const_major}
#define {const_fullname}
");

            mm.Log("[OTAPI] Done.");
        }
    }
}
