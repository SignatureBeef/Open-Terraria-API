#pragma warning disable CS0626

namespace Terraria
{
    class patch_WindowsLaunch //: Terraria.WindowsLaunch
    {
        private static extern void orig_Main(string[] args);
        private static void Main(string[] args)
        {
            orig_Main(args);
        }
        public static void LaunchGame(string[] args)
        {
            orig_Main(args);
        }
    }
}