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
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using ModFramework;
using ModFramework.Plugins;
using ModFramework.Relinker;
using Mono.Cecil;

namespace OTAPI.Patcher.Targets
{
    [MonoMod.MonoModIgnore]
    public class OTAPIServerTarget : IPatchTarget
    {
        public string DisplayText { get; } = "OTAPI Server";

        bool CanLoadFile(string filepath)
        {
            // only load "server" or "both" variants
            var filename = Path.GetFileNameWithoutExtension(filepath);
            return !filename.EndsWith(".Client", StringComparison.CurrentCultureIgnoreCase);
        }

        public void Patch()
        {
            Console.WriteLine($"Open Terraria API v{Common.GetVersion()}");

            PreShimForCompilation();
            ApplyModifications();
        }

        #region Produce OTAPI
        public void ApplyModifications()
        {

            //var freshAssembly = "../../../../OTAPI.Setup/bin/Debug/net5.0/TerrariaServer.dll";
            var localPath = "TerrariaServer.dll";

            //if (File.Exists(localPath)) File.Delete(localPath);
            //File.Copy(freshAssembly, localPath);

            // load into the current app domain for patch refs
            var asm = Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, localPath));
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                if (args.Name.IndexOf("TerrariaServer") > -1)
                {
                    return asm;
                }
                return null;
            };

            Console.WriteLine("[OTAPI] Extracting embedded binaries for assembly resolution...");
            var extractor = new ResourceExtractor();
            var embeddedResourcesDir = extractor.Extract(localPath);

            // load modfw plugins. this will load ModFramework.Modules and in turn top level c# scripts
            PluginLoader.AssemblyFound += CanLoadFile;
            ModFramework.Modules.CSharpLoader.AssemblyFound += CanLoadFile;
            ModFramework.Modules.CSharpLoader.GlobalAssemblies.Add(localPath);
            PluginLoader.TryLoad();

            Directory.CreateDirectory("outputs");

            var assembly_output = Path.Combine("outputs", "OTAPI.dll");
            var runtime_output = Path.Combine("outputs", "OTAPI.Runtime.dll");

            //var assembly_output = "OTAPI.dll";
            //var runtime_output = "OTAPI.Runtime.dll";

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

            mm.ReadMod(this.GetType().Assembly.Location);

            mm.AutoPatch();

#if tModLoaderServer_V1_3
                mm.WriterParameters.SymbolWriterProvider = null;
                mm.WriterParameters.WriteSymbols = false;
#endif

            {
                var sac = mm.Module.ImportReference(typeof(AssemblyInformationalVersionAttribute).GetConstructors()[0]);
                var sa = new CustomAttribute(sac);
                sa.ConstructorArguments.Add(new CustomAttributeArgument(mm.Module.TypeSystem.String, Common.GetVersion()));
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

            PluginLoader.Clear();
            CoreLibRelinker.PostProcessCoreLib(assembly_output, runtime_output);

            mm.Log("[OTAPI] Building NuGet package...");
            BuildNuGetPackage();

            mm.Log("[OTAPI] Done.");
        }

        static void BuildNuGetPackage()
        {
            const string packageFile = "OTAPI.nupkg";

            var nuspec_xml = File.ReadAllText("../../../../OTAPI.nuspec");
            nuspec_xml = nuspec_xml.Replace("[INJECT_VERSION]", Common.GetVersion());

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
        #endregion

        #region Produce TerrariaServer.dll (shimmed, vanilla)

        const String TerrariaWebsite = "https://terraria.org";

        public void PreShimForCompilation()
        {
            var input = DownloadServer();

            Console.WriteLine("[OTAPI] Extracting embedded binaries and packing into one binary...");

            // allow for refs to the embedded resources, such as ReLogic.dll
            var extractor = new ResourceExtractor();
            var embeddedResourcesDir = extractor.Extract(input);
            var inputName = Path.GetFileNameWithoutExtension(input);

            Directory.CreateDirectory("outputs");

            var output = Path.Combine("outputs", "TerrariaServer.dll");

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
            (mm.AssemblyResolver as DefaultAssemblyResolver)!.AddSearchDirectory(embeddedResourcesDir);
            mm.Read();

            // for HookResult + HookEvent
            mm.ReadMod(this.GetType().Assembly.Location);

            var initialModuleName = mm.Module.Name;

            // prechange the assembly name to a dll
            // monomod will also reference this when relinking so it must be correct
            // in order for shims within this dll to work (relogic)
            mm.Module.Name = "TerrariaServer.dll";
            mm.Module.Assembly.Name.Name = "TerrariaServer";

            foreach (var path in new[] {
                Path.Combine(System.Environment.CurrentDirectory, "TerrariaServer.OTAPI.Shims.mm.dll"),
                //Path.Combine(System.Environment.CurrentDirectory, "ModFramework.dll"),
                Directory.GetFiles(embeddedResourcesDir).Single(x => Path.GetFileName(x).Equals("ReLogic.dll", StringComparison.CurrentCultureIgnoreCase)),
                Directory.GetFiles(embeddedResourcesDir).Single(x => Path.GetFileName(x).Equals("Steamworks.NET.dll", StringComparison.CurrentCultureIgnoreCase)),
            })
            {
                mm.ReadMod(path);
            }

            // add the SourceAssembly name attribute
            {
                var sac = mm.Module.ImportReference(typeof(SourceAssemblyAttribute).GetConstructor(Type.EmptyTypes));
                var sa = new CustomAttribute(sac);
                sa.Fields.Add(new Mono.Cecil.CustomAttributeNamedArgument("ModuleName", new CustomAttributeArgument(mm.Module.TypeSystem.String, initialModuleName)));
                sa.Fields.Add(new Mono.Cecil.CustomAttributeNamedArgument("FileName", new CustomAttributeArgument(mm.Module.TypeSystem.String, inputName)));
                mm.Module.Assembly.CustomAttributes.Add(sa);
            }

            mm.MapDependencies();
            mm.AddTask(new CoreLibRelinker());
            mm.AutoPatch();

            //mm.OutputPath = mm.Module.Name; // the merged TerrariaServer + ReLogic (so we can apply patches)

            // switch to any cpu so that we can compile and use types in mods
            // this is usually in a modification otherwise
            mm.Module.Architecture = TargetArchitecture.I386;
            mm.Module.Attributes = ModuleAttributes.ILOnly;

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

            const string script_refs = "refs.dll";
            if (File.Exists(script_refs)) File.Delete(script_refs);
            File.Copy(output, script_refs);
        }

        public string DownloadZip(string url)
        {
            this.Log($"Downloading {url}");
            var uri = new Uri(url);
            string filename = Path.GetFileName(uri.AbsolutePath);
            if (!String.IsNullOrWhiteSpace(filename))
            {
                var savePath = Path.Combine(Environment.CurrentDirectory, filename);

                if (!File.Exists(savePath))
                {
                    using var client = new HttpClient();
                    var data = client.GetByteArrayAsync(url).Result;
                    File.WriteAllBytes(savePath, data);
                }

                return savePath;
            }
            else throw new NotSupportedException();
        }

        public string ExtractZip(string zipPath)
        {
            var directory = Path.GetFileNameWithoutExtension(zipPath);
            var info = new DirectoryInfo(directory);
            this.Log($"Extracting to {directory}");

            if (info.Exists) info.Delete(true);

            info.Refresh();

            if (!info.Exists || info.GetDirectories().Length == 0)
                ZipFile.ExtractToDirectory(zipPath, directory);

            return directory;
        }

        public string DownloadServer()
        {
            var zipUrl = GetZipUrl();
            var zipPath = DownloadZip(zipUrl);
            var extracted = ExtractZip(zipPath);

            return DetermineInputAssembly(extracted);
        }

        public string AquireLatestBinaryUrl()
        {
            this.Log("Determining the latest TerrariaServer.exe...");
            using var client = new HttpClient();

            var data = client.GetByteArrayAsync(TerrariaWebsite).Result;
            var html = System.Text.Encoding.UTF8.GetString(data);

            const String Lookup = ">PC Dedicated Server";

            var offset = html.IndexOf(Lookup, StringComparison.CurrentCultureIgnoreCase);
            if (offset == -1) throw new NotSupportedException();

            var attr_character = html[offset - 1];

            var url = html.Substring(0, offset - 1);
            var url_begin_offset = url.LastIndexOf(attr_character);
            if (url_begin_offset == -1) throw new NotSupportedException();

            url = url.Remove(0, url_begin_offset + 1);

            return TerrariaWebsite + url;
        }

        public string DetermineInputAssembly(string extractedFolder)
        {
            return Directory.EnumerateFiles(extractedFolder, "TerrariaServer.exe", SearchOption.AllDirectories).Single(x =>
                Path.GetFileName(Path.GetDirectoryName(x)).Equals("Windows", StringComparison.CurrentCultureIgnoreCase)
            );
        }

        public string GetZipUrl()
        {
            var cli = this.GetCliValue("latestVanilla");

            if (cli != "n")
            {
                int attempts = 5;
                do
                {
                    Console.Write("Download the latest binary? y/[N]: ");

                    var input = Console.ReadLine().ToLower();

                    if (input.Equals("y", StringComparison.CurrentCultureIgnoreCase))
                        return AquireLatestBinaryUrl();
                    else if (input.Equals("n", StringComparison.CurrentCultureIgnoreCase))
                        break;

                    if (String.IsNullOrWhiteSpace(input)) // no key entered
                        break;

                } while (attempts-- > 0);
            }

            return "https://terraria.org/system/dedicated_servers/archives/000/000/046/original/terraria-server-1423.zip";
        }
        #endregion
    }
}
