#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Chest break received data.
        /// </summary>
        public struct ChestBreakReceived
        {
            /// <summary>
            /// The X coordinate of the chest
            /// </summary>
            public int X { get; set; }

            /// <summary>
            /// The Y coordinate of the chest
            /// </summary>
            public int Y { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Triggered when a chest break packet is received.
        /// </summary>
        /// <description>
        /// This hook has multiple states:
        ///     1) IGNORE - disregard the change
        ///     2) RECTIFY - used to ignore the change, but to also refresh the sender with the initial tile square.
        ///     3) KICK - commonly used with HookContext.SetKick, to remove the player from the server.
        /// </description>
        public static readonly HookPoint<HookArgs.ChestBreakReceived> ChestBreakReceived = new HookPoint<HookArgs.ChestBreakReceived>();
    }
}
#endif