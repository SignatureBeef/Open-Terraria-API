using Mono.Cecil;
using MonoMod;
using MonoMod.RuntimeDetour.HookGen;
using OTAPI.Common;
using System;
using System.IO;
using System.Linq;

namespace OTAPI.Setup
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = Remote.DownloadServer();

            Console.WriteLine($"[OTAPI] Extracting embedded binaries and packing into one binary...");

            // allow for refs to the embedded resources, such as ReLogic.dll
            var extractor = new ResourceExtractor();
            var embeddedResourcesDir = extractor.Extract(input);
            var inputName = Path.GetFileNameWithoutExtension(input);

            var output = $"MMHOOK_{inputName}.dll";
            using (MonoModder mm = new MonoModder()
            {
                InputPath = input,
                OutputPath = output,
                ReadingMode = ReadingMode.Deferred,
                MissingDependencyThrow = false,
                PublicEverything = true, // we want all of terraria exposed

                GACPaths = new string[] { } // avoid MonoMod looking up the GAC, which causes an exception on .netcore
            })
            {
                (mm.AssemblyResolver as DefaultAssemblyResolver).AddSearchDirectory(embeddedResourcesDir);
                mm.Read();

                foreach (var path in new[] {
                    Path.Combine(System.Environment.CurrentDirectory, "TerrariaServer.OTAPI.Shims.mm.dll"),
                    Directory.GetFiles(embeddedResourcesDir).Single(x => Path.GetFileName(x).Equals("ReLogic.dll", StringComparison.CurrentCultureIgnoreCase)),
                })
                {
                    mm.Log($"[MonoMod] Reading mod or directory: {path}");
                    mm.ReadMod(path);
                }

                mm.MapDependencies();
                mm.AutoPatch();

                if (File.Exists(output))
                {
                    mm.Log($"[HookGen] Clearing {output}");
                    File.Delete(output);
                }

                mm.Log("[HookGen] Starting HookGenerator");
                var gen = new HookGenerator(mm, Path.GetFileName(output));
                using (ModuleDefinition mOut = gen.OutputModule)
                {
                    gen.Generate();

                    mOut.Write(output);
                }

                mm.OutputPath = $"{inputName}.dll"; // the merged TerrariaServer + ReLogic (so we can apply patches)

                // switch to any cpu so that we can compile and use types in mods
                // this is usually in a modification otherwise
                mm.Module.Architecture = TargetArchitecture.I386;
                mm.Module.Attributes = ModuleAttributes.ILOnly;

                mm.Write();

                mm.Log("[HookGen] Done.");
            }
        }
    }
}
