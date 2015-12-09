#if CLIENT
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct ChestSetupShop
        {
            public MethodState State { get; set; }

            public Terraria.Chest Chest { get; set; }

            public int Type { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.ChestSetupShop> ChestSetupShop = new HookPoint<HookArgs.ChestSetupShop>();
    }
}
#endif