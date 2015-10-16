#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Name conflict data.
        /// </summary>
        public struct NameConflict
        {
            /// <summary>
            /// Connecting player instance
            /// </summary>
            public Terraria.Player Connectee { get; set; }

            /// <summary>
            /// Connecting players buffer id
            /// </summary>
            public int BufferId { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Occurs when a player is connecting, but another player is already online with the same name.
        /// </summary>
        /// <description>
        /// Set the HookContext Result to anything but DEFAUL to allow the player
        /// You can also kick the player using the HookContext, by default vanilla will kick the connectee.
        /// </description>
        public static readonly HookPoint<HookArgs.NameConflict> NameConflict = new HookPoint<HookArgs.NameConflict>();
    }
}
#endif