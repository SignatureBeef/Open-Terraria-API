#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Player left game data.
        /// </summary>
        public struct PlayerLeftGame
        {
            /// <summary>
            /// The disconnecting players slot.
            /// </summary>
            public int Slot { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Occurs when a player has disconnected from the server.
        /// </summary>
        /// <description>
        /// This is mainly used for cleaning up, or notifications.
        /// </description>
        public static readonly HookPoint<HookArgs.PlayerLeftGame> PlayerLeftGame = new HookPoint<HookArgs.PlayerLeftGame>();
    }
}
#endif