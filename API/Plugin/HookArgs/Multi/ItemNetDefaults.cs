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
}
#endif