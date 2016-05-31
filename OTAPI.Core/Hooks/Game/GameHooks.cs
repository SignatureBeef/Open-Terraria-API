using System;

namespace OTAPI.Core
{
    public static partial class Hooks
    {
        public static partial class Game
        {
            /// <summary>
            /// Occurs at the first point of call when the Initialze method is ran.
            /// </summary>
            public static Action PreInitialize;

            /// <summary>
            /// Occurs at the end of the games initialize method.
            /// </summary>
            public static Action PostInitialize;

            /// <summary>
            /// Occurs at the first point of call when the game is running the update loop.
            /// </summary>
            public static Action PreUpdate;

            /// <summary>
            /// Occurs at the end of the games update loop.
            /// </summary>
            public static Action PostUpdate;
        }
    }
}
