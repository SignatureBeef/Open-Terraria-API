#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct DoorStateChanged
        {
            /// <summary>
            /// Location of the player when the state was changed
            /// </summary>
            /// <value>The position.</value>
            public Microsoft.Xna.Framework.Vector2 Position { get; set; }

            public int X { get; set; }
            public int Y { get; set; }

            public int Direction { get; set; }

            /// <summary>
            /// Type of object being opened
            /// </summary>
            /// <value>The kind.</value>
            public int Kind { get; set; }
        }
    }
}
#endif