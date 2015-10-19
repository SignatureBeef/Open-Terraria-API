#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Connection state data
        /// </summary>
        public struct CheckBufferState
        {
            /// <summary>
            /// The current buffer id that is about to receive data.
            /// </summary>
            /// <value>The buffer identifier.</value>
            public int BufferId { get; set; }

            /// <summary>
            /// The packet id being received
            /// </summary>
            public byte PacketId { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Occurs when a client sends data and the server checks for the correct connection state.
        /// </summary>
        public static readonly HookPoint<HookArgs.CheckBufferState> CheckBufferState = new HookPoint<HookArgs.CheckBufferState>();
    }
}
#endif