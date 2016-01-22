using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct NpcUpdate
        {
            public MethodState State { get; set; }

            public Terraria.NPC Npc { get; set; }

            public int NpcIndex { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.NpcUpdate> NpcUpdate = new HookPoint<HookArgs.NpcUpdate>();
    }
}