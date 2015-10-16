#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Player pre greeting data.
        /// </summary>
        public struct PlayerPreGreeting
        {
            /// <summary>
            /// The slot of the new player
            /// </summary>
            public int Slot { get; set; }

            /// <summary>
            /// The MOTD to be sent to the new player
            /// </summary>
            public string Motd { get; set; }

            /// <summary>
            /// The MOTD colour.
            /// </summary>
            public Microsoft.Xna.Framework.Color MotdColour { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Called before the player has been announced to other players, allowing for modifications.
        /// </summary>
        /// <description>
        /// Setting the HookContext.Result to anything other than DEFAULT will prevent the MOTD and player list being sent to the new player.
        /// This is also a kickable hook, via the HookContext.SetKick method.
        /// </description>
        public static readonly HookPoint<HookArgs.PlayerPreGreeting> PlayerPreGreeting = new HookPoint<HookArgs.PlayerPreGreeting>();
    }
}
#endif