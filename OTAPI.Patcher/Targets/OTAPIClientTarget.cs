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
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using ModFramework;
using ModFramework.Modules.CSharp;
using ModFramework.Plugins;
using ModFramework.Relinker;
using Mono.Cecil;
using OTAPI.Common;

namespace OTAPI.Patcher.Targets
{
    [MonoMod.MonoModIgnore]
    public class OTAPIClientLightweightTarget : IPatchTarget
    {
        public class StatusUpdateArgs : EventArgs
        {
            public string Text { get; set; }
        }

        public string DisplayText { get; } = "OTAPI Client (lightweight)";

        private MarkdownDocumentor markdownDocumentor = new ModificationMdDocumentor();

        public event EventHandler<StatusUpdateArgs> StatusUpdate;

        public string? InstallPath { get; set; }

        bool CanLoadPatchFile(string filepath)
        {
            // only load "client" or "both" variants
            var filename = Path.GetFileNameWithoutExtension(filepath);
            return !filename.EndsWith(".Server", StringComparison.CurrentCultureIgnoreCase)
                && !filename.EndsWith("-Server", StringComparison.CurrentCultureIgnoreCase)
                && !filename.EndsWith("-Runtime", StringComparison.CurrentCultureIgnoreCase);
        }

        bool CanLoadCompilationFile(string filepath)
        {
            // only load "client" or "both" variants
            var filename = Path.GetFileNameWithoutExtension(filepath);
            return !filename.EndsWith(".Server", StringComparison.CurrentCultureIgnoreCase)
                && !filename.EndsWith("-Server", StringComparison.CurrentCultureIgnoreCase)
                && !filename.EndsWith("-Runtime", StringComparison.CurrentCultureIgnoreCase)
                && !filename.EndsWith(".Runtime", StringComparison.CurrentCultureIgnoreCase)
                && !filename.EndsWith("-Client", StringComparison.CurrentCultureIgnoreCase)
                && !filename.EndsWith(".Client", StringComparison.CurrentCultureIgnoreCase);
        }

        IEnumerable<string> XnaPaths => new[]
        {
            @"C:\Windows\Microsoft.NET\assembly\GAC_32\Microsoft.Xna.Framework\v4.0_4.0.0.0__842cf8be1de50553",
            @"C:\Windows\Microsoft.NET\assembly\GAC_32\Microsoft.Xna.Framework.Game\v4.0_4.0.0.0__842cf8be1de50553",
            @"C:\Windows\Microsoft.NET\assembly\GAC_32\Microsoft.Xna.Framework.Graphics\v4.0_4.0.0.0__842cf8be1de50553",
            @"C:\Windows\Microsoft.NET\assembly\GAC_32\Microsoft.Xna.Framework.Xact\v4.0_4.0.0.0__842cf8be1de50553",
        };

        bool TryLoad(string file, out Assembly assembly)
        {
            assembly = null;
            if (File.Exists(file))
            {
                try
                {
                    var abs = Path.GetFullPath(file);
                    var content = File.ReadAllBytes(abs);
                    assembly = AssemblyLoadContext.Default.LoadFromStream(new MemoryStream(content));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return assembly != null;
        }

        void SetStatus(string status)
        {
            StatusUpdate?.Invoke(this, new StatusUpdateArgs() { Text = status });
            Console.WriteLine(status);
        }

        void RemovePatcherFromCompilation(object instance, CSharpLoader.CompilationContextArgs args)
        {
            args.Context.Compilation = args.Context.Compilation.WithReferences(args.Context.Compilation.References.Where(r => r.Display.IndexOf("OTAPI.Patcher") == -1));
        }

        public void Patch()
        {
            Console.WriteLine($"Open Terraria API v{Common.GetVersion()} [lightweight]");

            SetStatus("Starting...");

            PluginLoader.AssemblyFound += CanLoadPatchFile;
            CSharpLoader.AssemblyFound += CanLoadPatchFile;
            ModFramework.Modules.ClearScript.ScriptManager.FileFound += CanLoadPatchFile;
            ModFramework.Modules.Lua.ScriptManager.FileFound += CanLoadPatchFile;

            CSharpLoader.OnCompilationContext += RemovePatcherFromCompilation;
            try
            {
                this.AddMarkdownFormatter();

                ClientInstallPath<IInstallDiscoverer> installDiscoverer;

                if (InstallPath is not null)
                    installDiscoverer = ClientHelpers.FromPath(InstallPath);
                else
                    installDiscoverer = ClientHelpers.DetermineClientInstallPath();

                //var installDiscoverer = ClientHelpers.DetermineClientInstallPath();
                var installPath = installDiscoverer.Path;

                var input_regular = installDiscoverer.GetResource("Terraria.exe");
                var input_orig = installDiscoverer.GetResource("Terraria.orig.exe");

                var input = File.Exists(input_orig) ? input_orig : input_regular;

                var is_input_pristine = installDiscoverer.Target.VerifyIntegrity(input);
                if (!is_input_pristine)
                {
                    var fg = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine("Your vanilla install is not pristine and may likely cause patching issues.");
                    Console.Error.WriteLine("Please verify integrity of your Terraria installation or reinstall it.");
                    Console.Error.WriteLine("Support will not be given unless you are running a clean environment and can replicate the problem consistently.");
                    Console.Error.WriteLine("Continuing in 5 seconds.");
                    Console.ForegroundColor = fg;
                    System.Threading.Thread.Sleep(1000 * 5);
                }

                //var freshAssembly = "../../../../OTAPI.Setup/bin/Debug/net5.0/Terraria.exe";
                var localPath_x86 = "Terraria.x86.exe";
                var localPath_x64 = "Terraria.x64.exe";

                if (File.Exists(localPath_x86)) File.Delete(localPath_x86);
                if (File.Exists(localPath_x64)) File.Delete(localPath_x64);
                if (File.Exists("OTAPI.exe")) File.Delete("OTAPI.exe");

                File.Copy(input, localPath_x86);

                // bring across some file from the installation so mono.cecil/mod can find them
                foreach (var lib in new[]
                {
                    //"FNA.dll", use our custom FNA, otherwise in publish/release the assemblies will mismatch
                    "SteelSeriesEngineWrapper.dll",
                    "CSteamworks.dll",
                    "CUESDK_2015.dll",
                    "steam_api.dll",
                    "ReLogic.Native",
                    "LogitechLedEnginesWrapper.dll",
                    "nfd.dll",
                    //"../MacOS/osx/CSteamworks",
                })
                {
                    var name = Path.GetFileName(lib);
                    var src = installDiscoverer.GetResource(lib);
                    if (File.Exists(src))
                    {
                        if (File.Exists(name)) File.Delete(name);
                        File.Copy(src, name);
                    }
                }

                // needed for below resolutions
                Console.WriteLine("[OTAPI] Extracting embedded binaries for assembly resolution...");
                var extractor = new ResourceExtractor();
                var embeddedResourcesDir = extractor.Extract(localPath_x86);

                var FNA = "FNA.dll";
                //var asmFNA = Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, FNA));
                var fnaPath = Path.Combine(Environment.CurrentDirectory, FNA);
                if (!TryLoad(fnaPath, out Assembly asmFNA)) throw new Exception($"Failed to load: {fnaPath}");
                var assemblies = new Dictionary<string, Assembly>()
                {
                    {asmFNA.FullName, asmFNA },
                };
                Assembly PatchResolve(object sender, ResolveEventArgs args)
                {
                    Console.WriteLine("[Patch Resolve] " + args.Name);

                    var match = assemblies.FirstOrDefault(a => a.Key == args.Name);
                    if (match.Key != null)
                        return match.Value;

                    var asn = new AssemblyName(args.Name);
                    var filename = $"{asn.Name}.dll";
                    if (TryLoad(filename, out Assembly assembly))
                    {
                        assemblies.Add(assembly.FullName, assembly);
                        return assembly;
                    }

                    filename = Path.Combine(embeddedResourcesDir, $"{asn.Name}.dll");
                    if (TryLoad(filename, out Assembly resassembly))
                    {
                        assemblies.Add(resassembly.FullName, resassembly);
                        return resassembly;
                    }

                    foreach (var dir in XnaPaths)
                    {
                        var xnaDll = Path.Combine(dir, $"{asn.Name}.dll");
                        if (File.Exists(xnaDll))
                        {
                            return asmFNA;
                        }
                    }
                    return null;
                }
                AppDomain.CurrentDomain.AssemblyResolve += PatchResolve;

                var primaryAssemblyPath = Path.Combine(Environment.CurrentDirectory, localPath_x86);

                // hot patch terraria.exe straight up to x64 so we dont fail on Assembly.LoadFile next

                SetStatus("Converting to x64");
                {
                    using var pa = AssemblyDefinition.ReadAssembly(primaryAssemblyPath);
                    pa.MainModule.Architecture = TargetArchitecture.I386;
                    pa.MainModule.Attributes = ModuleAttributes.ILOnly;

                    if (installDiscoverer.Target.GetClientPlatform() == OSPlatform.Windows)
                    {
                        foreach (var dir in XnaPaths)
                        {
                            if (Directory.Exists(dir))
                                (pa.MainModule.AssemblyResolver as DefaultAssemblyResolver)!.AddSearchDirectory(dir);
                        }
                    }

                    foreach (var asmref in pa.MainModule.AssemblyReferences.ToArray())
                    {
                        if (asmref.Name.Contains("Microsoft.Xna.Framework"))
                        {
                            asmref.Name = "FNA";
                            asmref.PublicKey = null;
                            asmref.PublicKeyToken = null;
                            asmref.Version = typeof(Microsoft.Xna.Framework.Game).Assembly.GetName().Version;
                        }
                    }

                    primaryAssemblyPath = Path.Combine(Environment.CurrentDirectory, localPath_x64);
                    pa.Write(primaryAssemblyPath);
                }

                // load into the current app domain for patch refs
                //var asm = Assembly.LoadFile(primaryAssemblyPath);
                if (!TryLoad(primaryAssemblyPath, out Assembly asm)) throw new Exception($"Failed to load: {primaryAssemblyPath}");
                assemblies.Add(asm.FullName, asm);

                var resourcesPath = installDiscoverer.GetResourcePath();

                // 20210723 new install method, no need for this. this is also really bad to do, as vanilla installation will be ruined.
                //// if FNA.dll exists in the installation, remove it.
                //// this is so that ours locally is found using Mono.Cecil
                //// and ours will also install over the top again
                //var fna_res = installDiscoverer.GetResource("FNA.dll");
                //if (File.Exists(fna_res))
                //{
                //    File.Delete(fna_res);
                //}

                SetStatus("Loading plugins...");

                // set the /patchtime path for client installs
                PluginLoader.Clear();
                CSharpLoader.GlobalRootDirectory = Path.Combine("patchtime", "csharp");
                CSharpLoader.GlobalAssemblies.Clear();

                // build shims
                PluginLoader.Init();
                var ldr = new CSharpLoader()
                    .SetAutoLoadAssemblies(true)
                    .SetMarkdownDocumentor(markdownDocumentor);

                var md = ldr.CreateMetaData();
                var shims = ldr.LoadModules(md, "shims").ToArray();

                using (var public_mm = new ModFwModder()
                {
                    InputPath = primaryAssemblyPath,
                    OutputPath = "OTAPI.dll",
                    MissingDependencyThrow = false,
                    //LogVerboseEnabled = true,
                    PublicEverything = true,

                    GACPaths = new string[] { }, // avoid MonoMod looking up the GAC, which causes an exception on .netcore

                    MarkdownDocumentor = markdownDocumentor,
                })
                {
                    var dar = (DefaultAssemblyResolver)public_mm.AssemblyResolver;
                    dar.AddSearchDirectory(embeddedResourcesDir);
                    dar.AddSearchDirectory(resourcesPath);

                    public_mm.Read();
                    public_mm.MapDependencies();
                    public_mm.ReadMod(this.GetType().Assembly.Location);
                    public_mm.ReadMod(Path.Combine(embeddedResourcesDir, "ReLogic.dll"));
                    public_mm.ReadMod(Path.Combine(embeddedResourcesDir, "RailSDK.Net.dll"));

                    foreach (var path in shims)
                    {
                        public_mm.ReadMod(path);
                    }

                    // relink / merge into the output
                    public_mm.RelinkAssembly("ReLogic");
                    public_mm.RelinkAssembly("RailSDK.Net");

                    SetStatus("Merging and pregenerating files, this will be brief...");

                    public_mm.AutoPatch();
                    public_mm.Write();

                    //const string script_refs = "refs.dll";
                    //if (File.Exists(script_refs)) File.Delete(script_refs);
                    //File.Copy("OTAPI.dll", script_refs);

                    var inputName = Path.GetFileNameWithoutExtension(input_regular);
                    var initialModuleName = public_mm.Module.Name;

                    var version = public_mm.Module.Assembly.Name.Version;
                    var const_major = $"{inputName}_V{version.Major}_{version.Minor}";
                    var const_fullname = $"{inputName}_{version.ToString().Replace(".", "_")}";
                    var platform = "Platform_" + installDiscoverer.Target.GetClientPlatform().ToString();
                    var const_senddatapatch = $"{inputName}_SendDataNumber{(version >= new Version("1.4.3.0") ? "8" : "7")}";

                    File.WriteAllText("AutoGenerated.target", @$"<!-- DO NOT EDIT THIS FILE! It was auto generated by the setup project  -->
<Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
    <DefineConstants>{inputName};{const_major};{const_fullname};{platform};{const_senddatapatch}</DefineConstants>
  </PropertyGroup>
</Project>");
                    File.WriteAllText("AutoGenerated.cs", @$"#define {inputName}
#define {const_major}
#define {const_fullname}
#define {platform}
#define {const_senddatapatch}
");
                }

                PluginLoader.Clear();

                // load modfw plugins. this will load ModFramework.Modules and in turn top level c# scripts
                CSharpLoader.GlobalAssemblies.Add("OTAPI.dll");

                var fna = "FNA.dll";
                //var fna = installDiscoverer.GetResource("FNA.dll");
                if (File.Exists(fna)) CSharpLoader.GlobalAssemblies.Add(fna);
                PluginLoader.TryLoad();

                Directory.CreateDirectory("outputs");

                var temp_out = Path.Combine("outputs", "OTAPI.exe");

                SetStatus("Modifying installation, this will take a moment...");
                using (var mm = new ModFwModder()
                {
                    InputPath = "OTAPI.dll",
                    OutputPath = temp_out,
                    MissingDependencyThrow = false,
                    //LogVerboseEnabled = true,
                    //PublicEverything = true,

                    GACPaths = new string[] { }, // avoid MonoMod looking up the GAC, which causes an exception on .netcore

                    MarkdownDocumentor = markdownDocumentor,
                })
                {
                    (mm.AssemblyResolver as DefaultAssemblyResolver)!.AddSearchDirectory(embeddedResourcesDir);
                    (mm.AssemblyResolver as DefaultAssemblyResolver)!.AddSearchDirectory(resourcesPath);

                    mm.Read();

                    mm.MapDependencies();

                    mm.RelinkAssembly("System.Windows.Forms");
                    mm.RelinkAssembly("ReLogic");

                    this.AddPatchMetadata(mm);
                    this.AddEnvMetadata(mm);

                    mm.AddVersion(Common.GetVersion());

                    mm.AutoPatch();

#if tModLoaderServer_V1_3
                mm.WriterParameters.SymbolWriterProvider = null;
                mm.WriterParameters.WriteSymbols = false;
#endif

                    foreach (var asmref in mm.Module.AssemblyReferences.ToArray())
                    {
                        if (asmref.Name.Contains("System.Private.CoreLib") || asmref.Name.Contains("netstandard")
                            || asmref.Name.Contains("System.Windows.Forms"))
                        {
                            //mm.Module.AssemblyReferences.Remove(asmref);
                        }
                        else if (asmref.Name.Contains("Microsoft.Xna.Framework"))
                        {
                            asmref.Name = "FNA";
                            asmref.PublicKey = null;
                            asmref.PublicKeyToken = null;
                            asmref.Version = asmFNA.GetName().Version;
                        }
                    }

                    foreach (var mmt in mm.Module.Types.Where(x => x.Namespace == "MonoMod").ToArray())
                    {
                        mm.Module.Types.Remove(mmt);
                    }

                    mm.Write();

                    PluginLoader.Clear();

                    CreateRuntimeEvents();

                    SetStatus("Relinking to .NET5...");
                    CoreLibRelinker.PostProcessCoreLib(temp_out, "outputs/OTAPI.Runtime.dll");

                    SetStatus("Writing MD...");
                    var doco_md = $"OTAPI.PC.Client.${installDiscoverer.Target.GetClientPlatform()}.mfw.md";
                    if (File.Exists(doco_md)) File.Delete(doco_md);
                    markdownDocumentor.Write(doco_md);
                    markdownDocumentor.Dispose();

                    AppDomain.CurrentDomain.AssemblyResolve -= PatchResolve;

                    mm.Log("[OTAPI] Patching completed.");
                }

                if (File.Exists(localPath_x86)) File.Delete(localPath_x86);
                if (File.Exists(localPath_x64)) File.Delete(localPath_x64);
                if (File.Exists("OTAPI.dll")) File.Delete("OTAPI.dll");
            }
            finally
            {
                PluginLoader.AssemblyFound -= CanLoadPatchFile;
                CSharpLoader.AssemblyFound -= CanLoadPatchFile;
                ModFramework.Modules.ClearScript.ScriptManager.FileFound -= CanLoadPatchFile;
                ModFramework.Modules.Lua.ScriptManager.FileFound -= CanLoadPatchFile;
                CSharpLoader.OnCompilationContext -= RemovePatcherFromCompilation;
            }

            CompileModules();
            InsallModules();
        }

        void CompileModules()
        {
            SetStatus("Compiling modules...");
            Console.WriteLine("[OTAPI] Compiling modules.");
            PluginLoader.AssemblyFound += CanLoadCompilationFile;
            CSharpLoader.AssemblyFound += CanLoadCompilationFile;
            ModFramework.Modules.ClearScript.ScriptManager.FileFound += CanLoadCompilationFile;
            ModFramework.Modules.Lua.ScriptManager.FileFound += CanLoadCompilationFile;
            CSharpLoader.OnCompilationContext += RemovePatcherFromCompilation;
            try
            {
                PluginLoader.Clear();
                CSharpLoader.GlobalRootDirectory = Path.Combine("patchtime", "csharp");
                CSharpLoader.GlobalAssemblies.Clear();
                CSharpLoader.GlobalAssemblies.Add("OTAPI.exe");
                CSharpLoader.GlobalAssemblies.Add("OTAPI.Runtime.dll");
                CSharpLoader.GlobalAssemblies.Add("FNA.dll");
                PluginLoader.TryLoad();
                Modifier.Apply(ModType.Write);
            }
            finally
            {
                PluginLoader.AssemblyFound -= CanLoadCompilationFile;
                CSharpLoader.AssemblyFound -= CanLoadCompilationFile;
                ModFramework.Modules.ClearScript.ScriptManager.FileFound -= CanLoadCompilationFile;
                ModFramework.Modules.Lua.ScriptManager.FileFound -= CanLoadCompilationFile;
                CSharpLoader.OnCompilationContext -= RemovePatcherFromCompilation;
            }
        }

        void InsallModules()
        {
            var sources = Path.Combine(CSharpLoader.GlobalRootDirectory, "plugins", "modules-patched");
            var generated = Path.Combine(CSharpLoader.GlobalRootDirectory, "generated");

            foreach (var dir in Directory.GetDirectories(sources, "*", SearchOption.TopDirectoryOnly))
            {
                var mod_name = Path.GetFileName(dir);
                var generated_dll = Path.Combine(generated, $"CSharpScript_{mod_name}.dll");
                var generated_pdb = Path.Combine(generated, $"CSharpScript_{mod_name}.pdb");
                var generated_xml = Path.Combine(generated, $"CSharpScript_{mod_name}.xml");
                var resources = Path.Combine(sources, mod_name, $"Resources");

                var destination = Path.Combine("modifications", mod_name);
                var destination_dll = Path.Combine("modifications", mod_name, $"{mod_name}.dll");
                var destination_pdb = Path.Combine("modifications", mod_name, $"{mod_name}.pdb");
                var destination_xml = Path.Combine("modifications", mod_name, $"{mod_name}.xml");
                var destination_resources = Path.Combine("modifications", mod_name, "Resources");

                if (Directory.Exists(destination))
                    Directory.Delete(destination, true);

                Directory.CreateDirectory(destination);
                Directory.CreateDirectory(destination_resources);

                Utils.TransferFile(generated_dll, destination_dll);
                Utils.TransferFile(generated_pdb, destination_pdb);
                Utils.TransferFile(generated_xml, destination_xml);

                if (Directory.Exists(resources))
                    Utils.CopyFiles(resources, destination_resources);
            }
        }

        void CreateRuntimeEvents()
        {
            SetStatus("Creating runtime hooks...");
            PluginLoader.Clear();
            using (var mm = new ModFwModder()
            {
                InputPath = "outputs/OTAPI.exe",
                //OutputPath = "OTAPI.dll",
                MissingDependencyThrow = false,
                //LogVerboseEnabled = true,
                //PublicEverything = true,

                GACPaths = new string[] { } // avoid MonoMod looking up the GAC, which causes an exception on .netcore
            })
            {
                (mm.AssemblyResolver as DefaultAssemblyResolver)!.AddSearchDirectory("EmbeddedResources");

                mm.Read();
                mm.MapDependencies();

                //mm.Log("[OTAPI Client Install] Generating OTAPI.Runtime.dll");
                var gen = new MonoMod.RuntimeDetour.HookGen.HookGenerator(mm, "OTAPI.Runtime.dll");
                using (ModuleDefinition mOut = gen.OutputModule)
                {
                    gen.Generate();

                    foreach (var asmref in mOut.AssemblyReferences.ToArray())
                    {
                        if (asmref.Name.Contains("System.Private.CoreLib") || asmref.Name.Contains("netstandard"))
                        {
                            //mOut.AssemblyReferences.Remove(asmref);
                        }
                    }

                    //Directory.CreateDirectory("outputs");
                    mOut.Write("outputs/OTAPI.Runtime.dll");
                    //ModFramework.Relinker.MscorlibRelinker.PostProcessMscorLib("outputs/OTAPI.Runtime.dll");
                }
            }
        }
    }
}