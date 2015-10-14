#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct ItemSetDefaultsByType
        {
            public MethodState State { get; set; }

            public Terraria.Item Item { get; set; }
            public int Type { get; set; }
            public bool NoMatCheck { get; set; }
        }
    }
}
#endif