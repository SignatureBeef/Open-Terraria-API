#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Invasion npc spawn data.
        /// </summary>
        public struct InvasionNpcSpawn
        {
            /// <summary>
            /// Calling method state
            /// </summary>
            /// <value>The state.</value>
            public MethodState State { get; set; }

            /// <summary>
            /// X coordinate of the spawning NPC
            /// </summary>
            public int X { get; set; }

            /// <summary>
            /// Y coordinate of the spawning NPC
            /// </summary>
            public int Y { get; set; }

            /// <summary>
            /// NPC type
            /// </summary>
            public int Type { get; set; }

            /// <summary>
            /// Start of the NPC to start looking for a slot from
            /// </summary>
            public int Start { get; set; }

            public float AI0 { get; set; }

            public float AI1 { get; set; }

            public float AI2 { get; set; }

            public float AI3 { get; set; }

            /// <summary>
            /// The player index to pre-target
            /// </summary>
            /// <value>The target.</value>
            public int Target { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Triggered each time an invasion NPC is spawned.
        /// </summary>
        /// <description>
        /// This hook accepts values from HookContext.Result:
        ///     1) IGNORE - don't spawn the NPC
        ///     2) RECTIFY - Specify a custom npc id using HookContext.SetResult
        ///     3) anything else acts as DEFAULT - default vanilla functionality
        /// 
        /// Note, in regards to the End call args. The Start property is the newly spawned NPC's index.
        /// </description>
        public static readonly HookPoint<HookArgs.InvasionNpcSpawn> InvasionNpcSpawn = new HookPoint<HookArgs.InvasionNpcSpawn>();
    }
}
#endif