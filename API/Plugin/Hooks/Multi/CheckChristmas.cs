#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Check christmas data.
        /// </summary>
        public struct CheckChristmas
        {
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Triggered before vanilla runs the christmas checks.
        /// </summary>
        /// <description>
        /// To prevent vanilla touching the christmas flag, set HookContext.Result to anything but HookResult.DEFAULT
        /// </description>
        public static readonly HookPoint<HookArgs.CheckChristmas> CheckChristmas = new HookPoint<HookArgs.CheckChristmas>();
    }
}
#endif