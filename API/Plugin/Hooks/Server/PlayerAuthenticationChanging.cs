#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Player authentication changing.
        /// </summary>
        public struct PlayerAuthenticationChanging
        {
            /// <summary>
            /// The new authentication name the player is about to have applied
            /// </summary>
            public string AuthenticatedAs { get; set; }

            /// <summary>
            /// The source from which the player is being authenticated by
            /// </summary>
            public string AuthenticatedBy { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Occurs when a players authentication is about to be changed.
        /// </summary>
        /// <description>
        /// See the HookContext for the Player instance for the existing authentication if required.
        /// Setting HookContext.Result to anything but DEFAULT will cancel the change.
        /// </description>
        public static readonly HookPoint<HookArgs.PlayerAuthenticationChanging> PlayerAuthenticationChanging = new HookPoint<HookArgs.PlayerAuthenticationChanging>();
    }
}
#endif