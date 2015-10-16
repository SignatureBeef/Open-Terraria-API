#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// New connection data.
        /// </summary>
        public struct NewConnection
        {
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// When a new connection has began, and not started sending or reciving data.
        /// </summary>
        /// <description>
        /// HookContext.Connection holds the connecting socket.
        /// You are also able to kick the new connection at this state.
        /// </description>
        public static readonly HookPoint<HookArgs.NewConnection> NewConnection = new HookPoint<HookArgs.NewConnection>();
    }
}
#endif