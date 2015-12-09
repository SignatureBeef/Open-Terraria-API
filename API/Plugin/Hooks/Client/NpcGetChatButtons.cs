#if CLIENT
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct NpcGetChatButtons
        {
            public Terraria.NPC Npc { get; set; }

            public string[] Buttons { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.NpcGetChatButtons> NpcGetChatButtons = new HookPoint<HookArgs.NpcGetChatButtons>();
    }
}
#endif