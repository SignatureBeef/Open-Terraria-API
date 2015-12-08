#if SERVER || CLIENT
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Player entered game data
        /// </summary>
        public struct PlayerEnteredGame
        {
            /// <summary>
            /// The slot of the player
            /// </summary>
            public int Slot { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Occurs when a player has started playing in the server
        /// </summary>
        /// <description>
        /// This is a kickable hook, see HookContext.SetKick.
        /// See the HookContext for the player and connection instances
        /// </description>
        public static readonly HookPoint<HookArgs.PlayerEnteredGame> PlayerEnteredGame = new HookPoint<HookArgs.PlayerEnteredGame>();
    }
}
#endif