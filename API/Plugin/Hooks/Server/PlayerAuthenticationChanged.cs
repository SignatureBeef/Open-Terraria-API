#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Player authentication changed.
        /// </summary>
        public struct PlayerAuthenticationChanged
        {
            /// <summary>
            /// The authentication name the player is now known by
            /// </summary>
            public string AuthenticatedAs { get; set; }

            /// <summary>
            /// The source from which the player was authenticated by
            /// </summary>
            public string AuthenticatedBy { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Occurs when a players authenticationg has just changed
        /// </summary>
        public static readonly HookPoint<HookArgs.PlayerAuthenticationChanged> PlayerAuthenticationChanged = new HookPoint<HookArgs.PlayerAuthenticationChanged>();
    }
}
#endif