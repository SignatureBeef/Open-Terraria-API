#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Server state change data.
        /// </summary>
        public struct ServerStateChange
        {
            /// <summary>
            /// The new server state
            /// </summary>
            public ServerState ServerChangeState { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Occurs when the server changes states.
        /// </summary>
        public static readonly HookPoint<HookArgs.ServerStateChange> ServerStateChange = new HookPoint<HookArgs.ServerStateChange>();
    }
}
#endif