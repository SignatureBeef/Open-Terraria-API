#if CLIENT
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct NpcGetChat
        {
            public Terraria.NPC Npc { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.NpcGetChat> NpcGetChat = new HookPoint<HookArgs.NpcGetChat>();
    }
}
#endif