#if CLIENT
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct NpcFindFrame
        {
            public MethodState State { get; set; }

            public Terraria.NPC Npc { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.NpcFindFrame> NpcFindFrame = new HookPoint<HookArgs.NpcFindFrame>();
    }
}
#endif