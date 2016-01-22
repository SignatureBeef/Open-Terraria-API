#if CLIENT
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct NpcChatButtonClick
        {
            public Terraria.NPC Npc { get; set; }

            public OTA.Mod.Npc.NpcChatButton Button { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.NpcChatButtonClick> NpcChatButtonClick = new HookPoint<HookArgs.NpcChatButtonClick>();
    }
}
#endif