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
using System.IO;
using System.Linq;
using System.Net.Http;
using ModFramework;
using ModFramework.Plugins;
using Mono.Cecil;

namespace OTAPI.Setup.Targets
{
    [MonoMod.MonoModIgnore]
    public class VanillaClientPatchTarget : IPatchTarget
    {
        public string DisplayText { get; } = "Vanilla Client";

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

            valid &= Directory.Exists(macOS);
            valid &= Directory.Exists(resources);

            valid &= File.Exists(startScript);
            valid &= File.Exists(startBin);
            valid &= File.Exists(assembly);

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

        public void Patch()
        {
            var installPath = DetermineClientInstallPath();
            PatchOSXLaunch(installPath);

            var input = Path.Combine(installPath, "Resources/Terraria.exe");

            Directory.CreateDirectory("outputs");
            var output = Path.Combine("outputs", "Terraria.exe");

            var e = File.Exists(input);
            using ModFwModder mm = new ModFwModder()
            {
                InputPath = input,
                OutputPath = output,
                ReadingMode = ReadingMode.Deferred,
                MissingDependencyThrow = false,
                PublicEverything = true, // we want all of terraria exposed

                LogVerboseEnabled = false,

                GACPaths = new string[] { } // avoid MonoMod looking up the GAC, which causes an exception on .netcore
            };
            //(mm.AssemblyResolver as DefaultAssemblyResolver)!.AddSearchDirectory(embeddedResourcesDir);
            mm.Read();

            var inputName = Path.GetFileNameWithoutExtension(input);
            var initialModuleName = mm.Module.Name;

            //// add the SourceAssembly name attribute
            //{
            //    var sac = mm.Module.ImportReference(typeof(SourceAssemblyAttribute).GetConstructor(Type.EmptyTypes));
            //    var sa = new CustomAttribute(sac);
            //    sa.Fields.Add(new CustomAttributeNamedArgument("ModuleName", new CustomAttributeArgument(mm.Module.TypeSystem.String, initialModuleName)));
            //    sa.Fields.Add(new CustomAttributeNamedArgument("FileName", new CustomAttributeArgument(mm.Module.TypeSystem.String, inputName)));
            //    mm.Module.Assembly.CustomAttributes.Add(sa);
            //}

            //mm.ReadMod(typeof(Program).Assembly.Location);

            mm.MapDependencies();
            mm.AutoPatch();

            Console.WriteLine($"[OTAPI] Saving {mm.OutputPath}");
            mm.Write();

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

            PluginLoader.Clear();

            //// convert the libary to net5
            //CoreLibRelinker.PostProcessCoreLib(mm.OutputPath);
            var dest = Path.GetFileName(output);
            if (File.Exists(dest)) File.Delete(dest);
            File.Copy(output, dest);
        }

        void PatchOSXLaunch(string installPath)
        {
            {
                var launch_script = Path.Combine(installPath, "MacOS/Terraria");
                var backup_launch_script = Path.Combine(installPath, "MacOS/Terraria.bak.otapi");

                if (!File.Exists(backup_launch_script))
                {
                    File.Copy(launch_script, backup_launch_script);
                }

                var contents = File.ReadAllText(launch_script);
                var patched = contents.Replace("./Terraria.bin.osx $@", "./Terraria.patched.bin.osx $@");

                if (contents != patched)
                {
                    File.WriteAllText(launch_script, patched);
                }
            }

            {
                var bin = Path.Combine(installPath, "MacOS/Terraria.bin.osx");
                var patched_bin = Path.Combine(installPath, "MacOS/Terraria.patched.bin.osx");

                if (!File.Exists(patched_bin))
                {
                    File.Copy(bin, patched_bin);
                }
            }
        }
    }
}
