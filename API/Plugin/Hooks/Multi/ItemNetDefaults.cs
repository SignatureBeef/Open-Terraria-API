#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct ItemNetDefaults
        {
            public MethodState State { get; set; }

            public Terraria.Item Item { get; set; }

            public int Type { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.ItemNetDefaults> ItemNetDefaults = new HookPoint<HookArgs.ItemNetDefaults>();
    }
}
#endif