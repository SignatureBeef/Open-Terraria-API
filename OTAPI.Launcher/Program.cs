namespace OTAPI.Launcher
{
    class Program
    {
        static void Main(string[] args)
        {
            OTAPI.Hooks.Program.Launch = () =>
            {
                Terraria.Main.SkipAssemblyLoad = true;
                return HookResult.Continue;
            };
            Terraria.WindowsLaunch.Main(args);
        }
    }
}
