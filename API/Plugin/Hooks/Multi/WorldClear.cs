#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// World clear data.
        /// </summary>
        public struct WorldClear
        {
            /// <summary>
            /// The position of which the hook was called from the wrapped method.
            /// </summary>
            /// <value>The state.</value>
            public MethodState State { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// This is called at the start and end of the Terraria.WorldGen.clearWorld method, allowing you to add
        /// additional functions or to prevent vanilla clearing tiles.
        /// </summary>
        /// <description>
        /// The Begin state of the args is the only cancellable hook. To cancel you must set the HookContext.Result
        /// to anything but HookResult.DEFAULT.
        /// </description>
        public static readonly HookPoint<HookArgs.WorldClear> WorldClear = new HookPoint<HookArgs.WorldClear>();
    }
}
#endif