using System;
using System.IO;

namespace ModFramework.Modules.ClearScript
{
    public static class Hooks
    {
        public static ScriptManager ScriptManager { get; set; }

        [Modification(ModType.Runtime, "Loading ClearScript interface")]
        public static void OnRunning()
        {
            Launch();
        }

        [Modification(ModType.Read, "Loading ClearScript interface")]
        public static void OnModding(MonoMod.MonoModder modder)
        {
            Launch(modder);
        }

        static void Launch(MonoMod.MonoModder modder = null)
        {
            const string root = "clearscript";
            Directory.CreateDirectory(root);

            Console.WriteLine($"[JS] Loading ClearScript files from ./{root}");

            ScriptManager = new ScriptManager(root, modder);
            ScriptManager.Initialise();
            ScriptManager.WatchForChanges();
        }
    }
}
