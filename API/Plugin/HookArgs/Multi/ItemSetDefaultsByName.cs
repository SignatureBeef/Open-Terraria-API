#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct ItemSetDefaultsByName
        {
            public MethodState State { get; set; }

            public Terraria.Item Item { get; set; }
            public string Name { get; set; }
        }
    }
}
#endif