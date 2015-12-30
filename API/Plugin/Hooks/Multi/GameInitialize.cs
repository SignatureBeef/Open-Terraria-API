#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Game initialise data.
        /// </summary>
        public struct GameInitialize
        {
            /// <summary>
            /// Calling method state
            /// </summary>
            public MethodState State { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Triggered at the Begin and End of Terraria.Main.Initialize
        /// </summary>
        /// <description>
        /// See the relevent GameInitialize.State values for the call position
        /// </description>
        public static readonly HookPoint<HookArgs.GameInitialize> GameInitialize = new HookPoint<HookArgs.GameInitialize>();
    }
}
#endif