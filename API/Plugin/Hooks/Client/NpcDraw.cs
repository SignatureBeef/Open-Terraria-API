#if CLIENT
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct NpcDraw
        {
            public MethodState State { get; set; }

            public Terraria.NPC Npc { get; set; }

            public bool BehindTiles { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.NpcDraw> NpcDraw = new HookPoint<HookArgs.NpcDraw>();
    }
}
#endif