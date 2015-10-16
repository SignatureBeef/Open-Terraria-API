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

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.ItemSetDefaultsByName> ItemSetDefaultsByName = new HookPoint<HookArgs.ItemSetDefaultsByName>();
    }
}
#endif