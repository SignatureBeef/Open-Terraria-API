#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// NPC info
        /// </summary>
        public struct NewNpc
        {
            /// <summary>
            /// Type of NPC to be created
            /// </summary>
            public int Type { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Occurs when vanilla calls Terraria.NPC.NewNPC and a new NPC is to be created
        /// </summary>
        public static readonly HookPoint<HookArgs.NewNpc> NewNpc = new HookPoint<HookArgs.NewNpc>();
    }
}
#endif