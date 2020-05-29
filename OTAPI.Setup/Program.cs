using Mono.Cecil;
using MonoMod;
using MonoMod.RuntimeDetour.HookGen;
using OTAPI.Common;
using System.IO;

namespace OTAPI.Setup
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = Remote.DownloadServer();
            var output = $"MMHOOK_{Path.GetFileNameWithoutExtension(input)}.dll";
            using (MonoModder mm = new MonoModder()
            {
                InputPath = input,
                OutputPath = output,
                ReadingMode = ReadingMode.Deferred,
                MissingDependencyThrow = false,

                GACPaths = new string[] { } // avoid MonoMod looking up the GAC, which causes an exception on .netcore
            })
            {
                mm.Read();

                mm.MapDependencies();

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

                mm.Log("[HookGen] Done.");
            }
        }
    }
}
