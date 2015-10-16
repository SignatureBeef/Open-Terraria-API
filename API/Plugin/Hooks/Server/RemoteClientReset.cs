#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Remote client reset data.
        /// </summary>
        public struct RemoteClientReset
        {
            /// <summary>
            /// The instance being reset
            /// </summary>
            public Terraria.RemoteClient Client { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Occurs when a socket has been reset (via Terraria.RemoteClient.Reset)
        /// </summary>
        public static readonly HookPoint<HookArgs.RemoteClientReset> RemoteClientReset = new HookPoint<HookArgs.RemoteClientReset>();
    }
}
#endif