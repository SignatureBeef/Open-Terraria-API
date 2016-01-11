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

            public int NpcIndex { get; set; }

            public int X { get; set; }

            public int Y { get; set; }

            public int Start { get; set; }

            public float AI0 { get; set; }

            public float AI1 { get; set; }

            public float AI2 { get; set; }

            public float AI3 { get; set; }

            public int Target { get; set; }
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