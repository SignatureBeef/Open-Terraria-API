using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct NpcAI
        {
            public MethodState State { get; set; }

            public Terraria.NPC Npc { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.NpcAI> NpcAI = new HookPoint<HookArgs.NpcAI>();
    }
}