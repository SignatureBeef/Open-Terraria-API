#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct NpcNetDefaults
        {
            public MethodState State { get; set; }

            public Terraria.NPC Npc { get; set; }
            public int Type { get; set; }
        }
    }
}
#endif