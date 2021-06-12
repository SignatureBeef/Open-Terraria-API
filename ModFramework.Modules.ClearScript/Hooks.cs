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
            const string root = "clearscript";
            Directory.CreateDirectory(root);

            Console.WriteLine($"[JS] Loading ClearScript files from ./{root}");

            ScriptManager = new ScriptManager(root);
            ScriptManager.Initialise();
            ScriptManager.WatchForChanges();
        }
    }
}
