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

        bool CanLoadFile(string filepath)
        {
            // only load "client" or "both" variants
            var filename = Path.GetFileNameWithoutExtension(filepath);
            return !filename.EndsWith(".Server", StringComparison.CurrentCultureIgnoreCase)
                && !filename.EndsWith("-Server", StringComparison.CurrentCultureIgnoreCase);
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
                    assembly = Assembly.LoadFile(abs);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return assembly != null;
        }

        public void Patch()
        {
            Console.WriteLine($"Open Terraria API v{Common.GetVersion()} [lightweight]");

            StatusUpdate?.Invoke(this, new StatusUpdateArgs() { Text = "Starting..." });

            PluginLoader.AssemblyFound += CanLoadFile;
            CSharpLoader.AssemblyFound += CanLoadFile;
            ModFramework.Modules.ClearScript.ScriptManager.FileFound += CanLoadFile;
            ModFramework.Modules.Lua.ScriptManager.FileFound += CanLoadFile;

            var installDiscoverer = ClientHelpers.DetermineClientInstallPath();
            var installPath = installDiscoverer.Path;

            var input_regular = installDiscoverer.GetResource("Terraria.exe");
            var input_orig = installDiscoverer.GetResource("Terraria.orig.exe");

            var input = File.Exists(input_orig) ? input_orig : input_regular;

            //var freshAssembly = "../../../../OTAPI.Setup/bin/Debug/net5.0/Terraria.exe";
            var localPath_x86 = "Terraria.x86.exe";
            var localPath_x64 = "Terraria.x64.exe";

            if (File.Exists(localPath_x86)) File.Delete(localPath_x86);
            if (File.Exists(localPath_x64)) File.Delete(localPath_x64);

            File.Copy(input, localPath_x86);

            foreach (var lib in new[]
            {
                "FNA.dll",
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
            var asmFNA = Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, FNA));
            var assemblies = new Dictionary<string, Assembly>()
            {
                {asmFNA.FullName, asmFNA },
            };
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
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
            };

            var primaryAssemblyPath = Path.Combine(Environment.CurrentDirectory, localPath_x86);

            // hot patch terraria.exe straight up to x64 so we dont fail on Assembly.LoadFile next

            StatusUpdate?.Invoke(this, new StatusUpdateArgs() { Text = "Converting to x64" });
            {
                var pa = AssemblyDefinition.ReadAssembly(primaryAssemblyPath);
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
            var asm = Assembly.LoadFile(primaryAssemblyPath);
            assemblies.Add(asm.FullName, asm);

            var resourcesPath = installDiscoverer.GetResourcePath();

            StatusUpdate?.Invoke(this, new StatusUpdateArgs() { Text = "Loading plugins..." });

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
                (public_mm.AssemblyResolver as DefaultAssemblyResolver)!.AddSearchDirectory(embeddedResourcesDir);
                (public_mm.AssemblyResolver as DefaultAssemblyResolver)!.AddSearchDirectory(resourcesPath);

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

                StatusUpdate?.Invoke(this, new StatusUpdateArgs() { Text = "Merging and pregenerating files, this will be brief..." });

                public_mm.AutoPatch();
                public_mm.Write();

                const string script_refs = "refs.dll";
                if (File.Exists(script_refs)) File.Delete(script_refs);
                File.Copy("OTAPI.dll", script_refs);

                var inputName = Path.GetFileNameWithoutExtension(input_regular);
                var initialModuleName = public_mm.Module.Name;

                var const_major = $"{inputName}_V{public_mm.Module.Assembly.Name.Version.Major}_{public_mm.Module.Assembly.Name.Version.Minor}";
                var const_fullname = $"{inputName}_{public_mm.Module.Assembly.Name.Version.ToString().Replace(".", "_")}";
                var platform = "Platform_" + installDiscoverer.Target.GetClientPlatform().ToString();

                File.WriteAllText("AutoGenerated.target", @$"<!-- DO NOT EDIT THIS FILE! It was auto generated by the setup project  -->
<Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
    <DefineConstants>{inputName};{const_major};{const_fullname};{platform}</DefineConstants>
  </PropertyGroup>
</Project>");
                File.WriteAllText("AutoGenerated.cs", @$"#define {inputName}
#define {const_major}
#define {const_fullname}
#define {platform}
");
            }

            PluginLoader.Clear();

            // load modfw plugins. this will load ModFramework.Modules and in turn top level c# scripts
            CSharpLoader.GlobalAssemblies.Add("OTAPI.dll");

            var fna = installDiscoverer.GetResource("FNA.dll");
            if (File.Exists(fna)) CSharpLoader.GlobalAssemblies.Add(fna);
            PluginLoader.TryLoad();

            Directory.CreateDirectory("outputs");

            var temp_out = Path.Combine("outputs", "OTAPI.exe");


            StatusUpdate?.Invoke(this, new StatusUpdateArgs() { Text = "Modifying installation, this will take a moment..." });
            using var mm = new ModFwModder()
            {
                InputPath = "OTAPI.dll",
                OutputPath = temp_out,
                MissingDependencyThrow = false,
                //LogVerboseEnabled = true,
                //PublicEverything = true,

                GACPaths = new string[] { }, // avoid MonoMod looking up the GAC, which causes an exception on .netcore

                MarkdownDocumentor = markdownDocumentor,
            };
            (mm.AssemblyResolver as DefaultAssemblyResolver)!.AddSearchDirectory(embeddedResourcesDir);
            (mm.AssemblyResolver as DefaultAssemblyResolver)!.AddSearchDirectory(resourcesPath);

            mm.Read();

            mm.MapDependencies();

            mm.RelinkAssembly("System.Windows.Forms");

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
                    asmref.Version = new Version("21.5.0.0");
                }
            }

            foreach (var mmt in mm.Module.Types.Where(x => x.Namespace == "MonoMod").ToArray())
            {
                mm.Module.Types.Remove(mmt);
            }

            mm.Write();

            PluginLoader.Clear();

            CreateRuntimeEvents();

            StatusUpdate?.Invoke(this, new StatusUpdateArgs() { Text = "Relinking to .NET5..." });
            CoreLibRelinker.PostProcessCoreLib(temp_out, "outputs/OTAPI.Runtime.dll");

            StatusUpdate?.Invoke(this, new StatusUpdateArgs() { Text = "Writing MD..." });
            var doco_md = $"OTAPI.PC.Client.${installDiscoverer.Target.GetClientPlatform()}.mfw.md";
            if (File.Exists(doco_md)) File.Delete(doco_md);
            markdownDocumentor.Write(doco_md);
            markdownDocumentor.Dispose();

            mm.Log("[OTAPI] Done.");
        }

        void CreateRuntimeEvents()
        {
            Console.WriteLine("[OTAPI] Creating runtime events");

            PluginLoader.Clear();

            StatusUpdate?.Invoke(this, new StatusUpdateArgs() { Text = "Creating runtime hooks..." });
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
                //(mm.AssemblyResolver as DefaultAssemblyResolver)!.AddSearchDirectory(Path.GetDirectoryName(typeof(object).Assembly.Location));
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