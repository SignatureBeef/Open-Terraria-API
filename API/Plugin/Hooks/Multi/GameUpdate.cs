#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Game update data.
        /// </summary>
        public struct GameUpdate
        {
            /// <summary>
            /// Cached begin data
            /// </summary>
            /// <remarks>Since there is no useful data and is a high use event</remarks>
            public static readonly GameUpdate Begin = new GameUpdate() { State = MethodState.Begin };
            /// <summary>
            /// Cached end data
            /// </summary>
            /// <remarks>Since there is no useful data and is a high use event</remarks>
            public static readonly GameUpdate End = new GameUpdate() { State = MethodState.End };

            /// <summary>
            /// Calling method state
            /// </summary>
            public MethodState State { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Triggered at the Begin and End of Terraria.Main.Update
        /// </summary>
        /// <description>
        /// See the relevent GameUpdate.State values for the call position
        /// </description>
        public static readonly HookPoint<HookArgs.GameUpdate> GameUpdate = new HookPoint<HookArgs.GameUpdate>();
    }
}
#endif