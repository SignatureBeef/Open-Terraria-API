#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct NpcTransform
        {
            public MethodState State { get; set; }

            public Terraria.NPC Npc { get; set; }
            public int NewType { get; set; }
        }
    }
}
#endif