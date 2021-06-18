using System;
using System.IO;

namespace ModFramework.Modules.ClearScript
{
    public class Program
    {
        public static void Main()
        {
            Directory.CreateDirectory(Hooks.RootFolder);

            Console.WriteLine($"[JS] Loading ClearScript files from ./{Hooks.RootFolder}");

            var ScriptManager = new ScriptManager(Hooks.RootFolder, null);
            ScriptManager.Initialise();
            ScriptManager.WatchForChanges();
            ScriptManager.Cli();
        }
    }
}
