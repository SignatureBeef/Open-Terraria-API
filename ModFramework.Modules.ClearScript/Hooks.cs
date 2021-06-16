using System;
using System.IO;
using System.Reflection;
using ModFramework.Modules.ClearScript.Typings;

namespace ModFramework.Modules.ClearScript
{
    public static class Hooks
    {
        public const string RootFolder = "clearscript";

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
            Directory.CreateDirectory(RootFolder);

            Console.WriteLine($"[JS] Loading ClearScript files from ./{RootFolder}");

            ScriptManager = new ScriptManager(RootFolder, modder);
            ScriptManager.Initialise();
            ScriptManager.WatchForChanges();
        }
    }
}
