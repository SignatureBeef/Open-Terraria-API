#pragma warning disable CS0626
using OTAPI;
using System;

namespace Terraria
{
    class patch_Program
    {
        public static extern void orig_LaunchGame(string[] args, bool monoArgs = false);
        public static void LaunchGame(string[] args, bool monoArgs = false)
        {
            if (Hooks.Program.Launch == null || Hooks.Program.Launch() == HookResult.Continue)
            {
                orig_LaunchGame(args, monoArgs);
            }
        }
        public static extern void orig_DisplayException(Exception e);
        private static void DisplayException(Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}