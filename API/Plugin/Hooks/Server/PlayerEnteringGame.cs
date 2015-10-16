#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Player entering game data.
        /// </summary>
        public struct PlayerEnteringGame
        {
            /// <summary>
            /// The slot of the new player
            /// </summary>
            public int Slot { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Occurs before the server issues the new player's data to all other connected players.
        /// </summary>
        /// <description>This is a kickable hook, see HookContext.SetKick</description>
        public static readonly HookPoint<HookArgs.PlayerEnteringGame> PlayerEnteringGame = new HookPoint<HookArgs.PlayerEnteringGame>();
    }
}
#endif