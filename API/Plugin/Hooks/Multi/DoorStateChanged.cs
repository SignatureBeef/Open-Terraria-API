#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Door state changed data.
        /// </summary>
        public struct DoorStateChanged
        {
            /// <summary>
            /// Location of the player when the state was changed
            /// </summary>
            public Microsoft.Xna.Framework.Vector2 Position { get; set; }

            /// <summary>
            /// X coordinate
            /// </summary>
            public int X { get; set; }

            /// <summary>
            /// Y Coordinate
            /// </summary>
            public int Y { get; set; }

            /// <summary>
            /// The direction of the foor
            /// </summary>
            public int Direction { get; set; }

            /// <summary>
            /// Type of object being opened
            /// </summary>
            public int Kind { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Triggered when a player opens or closes a door.
        /// </summary>
        /// <description>
        /// This hook accepts HookContext.Result values of:
        ///     1) DEFAULT - to perform vanilla actions
        ///     2) RECTIFY - ignores changes, and reupdates the client
        ///     3) Anything else is considered as IGNORE, and is actionless.
        /// </description>
        public static readonly HookPoint<HookArgs.DoorStateChanged> DoorStateChanged = new HookPoint<HookArgs.DoorStateChanged>();
    }
}
#endif