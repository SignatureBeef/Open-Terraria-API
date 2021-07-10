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
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using ModFramework;
using ModFramework.Modules.CSharp;
using ModFramework.Plugins;
using ModFramework.Relinker;
using Mono.Cecil;

namespace OTAPI.Patcher.Targets
{
    [MonoMod.MonoModIgnore]
    public class OTAPIPCServerTarget : IPatchTarget
    {
        public const String TerrariaWebsite = "https://terraria.org";

        public virtual string DisplayText { get; } = "OTAPI PC Server";

        public virtual string CliKey { get; } = "latest";

        public virtual string NuGetPackageFileName { get; } = "OTAPI.PC.nupkg";
        public virtual string NuSpecFilePath { get; } = "../../../../OTAPI.PC.nuspec";
        public virtual string MdFileName { get; } = "OTAPI.PC.Server.mfw.md";

        public virtual string SupportedDownloadUrl { get; } = $"{TerrariaWebsite}/api/download/pc-dedicated-server/terraria-server-1423.zip";
        public virtual string ArtifactName { get; } = "artifact-pc";

        private MarkdownDocumentor markdownDocumentor = new ModificationMdDocumentor();

        protected virtual bool CanLoadFile(string filepath)
        {
            // only load "server" or "both" variants
            var filename = Path.GetFileNameWithoutExtension(filepath);
            return !filename.EndsWith(".Client", StringComparison.CurrentCultureIgnoreCase)
                && !filename.EndsWith("-Client", StringComparison.CurrentCultureIgnoreCase);
        }

        public virtual void Patch()
        {
            Console.WriteLine($"Open Terraria API v{Common.GetVersion()}");

            PluginLoader.AssemblyFound += CanLoadFile;
            ModFramework.Modules.CSharp.CSharpLoader.AssemblyFound += CanLoadFile;
            ModFramework.Modules.ClearScript.ScriptManager.FileFound += CanLoadFile;
            ModFramework.Modules.Lua.ScriptManager.FileFound += CanLoadFile;

            //markdownDocumentor.WriteLine += (ref string line, ref bool handled) =>
            //{
            //    if (line.Contains("@doc"))
            //    {
            //        line = line.Replace("@doc", "").Trim();
            //    }
            //};

            PreShimForCompilation();
            ApplyModifications();

            if (File.Exists(MdFileName)) File.Delete(MdFileName);
            markdownDocumentor.Write(MdFileName);
            markdownDocumentor.Dispose();

            this.WriteCIArtifacts(ArtifactName);
        }

        #region Produce OTAPI
        public void ApplyModifications()
        {
            var localPath = "./TerrariaServer.dll";

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
            ModFramework.Modules.CSharp.CSharpLoader.GlobalAssemblies.Add(localPath);
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

                GACPaths = new string[] { }, // avoid MonoMod looking up the GAC, which causes an exception on .netcore

                MarkdownDocumentor = markdownDocumentor,
            };
            (mm.AssemblyResolver as DefaultAssemblyResolver)!.AddSearchDirectory(embeddedResourcesDir);
            mm.Read();

            mm.MapDependencies();

            mm.ReadMod(this.GetType().Assembly.Location);

            this.AddPatchMetadata(mm);
            this.AddEnvMetadata(mm);

            mm.AutoPatch();

#if tModLoaderServer_V1_3
                mm.WriterParameters.SymbolWriterProvider = null;
                mm.WriterParameters.WriteSymbols = false;
#endif

            mm.AddVersion(Common.GetVersion());

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

        void BuildNuGetPackage()
        {
            var nuspec_xml = File.ReadAllText(NuSpecFilePath);
            nuspec_xml = nuspec_xml.Replace("[INJECT_VERSION]", Common.GetVersion());

            var commitSha = Common.GetGitCommitSha();
            nuspec_xml = nuspec_xml.Replace("[INJECT_GIT_HASH]", String.IsNullOrWhiteSpace(commitSha) ? "" : $" git#{commitSha}");

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

                if (File.Exists(NuGetPackageFileName))
                    File.Delete(NuGetPackageFileName);

                using (var srm = File.OpenWrite(NuGetPackageFileName))
                    packageBuilder.Save(srm);
            }
        }
        #endregion

        #region Produce TerrariaServer.dll (shimmed, vanilla)

        public virtual void PreShimForCompilation()
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

                GACPaths = new string[] { }, // avoid MonoMod looking up the GAC, which causes an exception on .netcore

                MarkdownDocumentor = markdownDocumentor,
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

            // build shims
            PluginLoader.Init();
            var ldr = new CSharpLoader()
                .SetAutoLoadAssemblies(true)
                .SetMarkdownDocumentor(markdownDocumentor);
            var md = ldr.CreateMetaData();
            var shims = ldr.LoadModules(md, "shims").ToArray();

            //var asd = "";

            foreach (var path in shims)
            {
                mm.ReadMod(path);
            }

            foreach (var path in new[] {
                //Path.Combine(System.Environment.CurrentDirectory, "TerrariaServer.OTAPI.Shims.mm.dll"),
                //Path.Combine(System.Environment.CurrentDirectory, "ModFramework.dll"),
                Directory.GetFiles(embeddedResourcesDir).Single(x => Path.GetFileName(x).Equals("ReLogic.dll", StringComparison.CurrentCultureIgnoreCase)),
                Directory.GetFiles(embeddedResourcesDir).Single(x => Path.GetFileName(x).Equals("Steamworks.NET.dll", StringComparison.CurrentCultureIgnoreCase)),
            })
            {
                mm.ReadMod(path);
            }

            mm.RelinkAssembly("ReLogic");
            mm.RelinkAssembly("Steamworks.NET");

            this.AddPatchMetadata(mm, initialModuleName, input);

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

        public virtual string DownloadZip(string url)
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

        public virtual string ExtractZip(string zipPath)
        {
            var directory = Path.GetFileNameWithoutExtension(zipPath);
            var info = new DirectoryInfo(directory);
            this.Log($"Extracting to {directory}");

            if (info.Exists) info.Delete(true);

            info.Refresh();

            if (!info.Exists || info.GetDirectories().Length == 0)
            {
                info.Create();
                ZipFile.ExtractToDirectory(zipPath, directory);
            }

            return directory;
        }

        public virtual string DownloadServer()
        {
            var zipUrl = GetZipUrl();
            var zipPath = DownloadZip(zipUrl);
            var extracted = ExtractZip(zipPath);

            return DetermineInputAssembly(extracted);
        }

        public virtual string GetUrlFromHttpResponse(string content)
        {
            var items = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(content);
            var server = items.Single(i => i.Contains("terraria-server-", StringComparison.OrdinalIgnoreCase));
            return $"{TerrariaWebsite}/api/download/pc-dedicated-server/{server}";
        }

        public virtual string AquireLatestBinaryUrl()
        {
            this.Log("Determining the latest TerrariaServer.exe...");
            using var client = new HttpClient();

            var data = client.GetByteArrayAsync($"{TerrariaWebsite}/api/get/dedicated-servers-names").Result;
            var json = System.Text.Encoding.UTF8.GetString(data);

            return GetUrlFromHttpResponse(json);
        }

        public virtual string DetermineInputAssembly(string extractedFolder)
        {
            return Directory.EnumerateFiles(extractedFolder, "TerrariaServer.exe", SearchOption.AllDirectories).Single(x =>
                Path.GetFileName(Path.GetDirectoryName(x)).Equals("Windows", StringComparison.CurrentCultureIgnoreCase)
            );
        }

        public virtual string GetZipUrl()
        {
            var cli = this.GetCliValue(CliKey);

            if (cli != "n")
            {
                int attempts = 5;
                do
                {
                    Console.Write("Download the latest binary? y/[N]: ");

                    //var input = Console.ReadLine().ToLower();
                    var input = Console.ReadKey(true).Key;
                    Console.WriteLine(input);

                    if (input == ConsoleKey.Y)
                        return AquireLatestBinaryUrl();
                    else if (input == ConsoleKey.N)
                        break;
                    else if (input == ConsoleKey.Enter)
                        break;
                } while (attempts-- > 0);
            }

            return SupportedDownloadUrl;
        }
        #endregion
    }
}
