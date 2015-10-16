#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct NpcSetDefaultsByName
        {
            public MethodState State { get; set; }

            public Terraria.NPC Npc { get; set; }

            public string Name { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.NpcSetDefaultsByName> NpcSetDefaultsByName = new HookPoint<HookArgs.NpcSetDefaultsByName>();
    }
}
#endif