#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct NpcStrike
        {
            public Terraria.NPC Npc { get; set; }
            public double Damage { get; set; }
        }
    }
}
#endif