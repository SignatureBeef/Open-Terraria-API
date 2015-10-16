#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Check halloween data.
        /// </summary>
        public struct CheckHalloween
        {
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Triggered before vanilla runs the halloween checks.
        /// </summary>
        /// <description>
        /// To prevent vanilla touching the halloween flag, set HookContext.Result to anything but HookResult.DEFAULT
        /// </description>
        public static readonly HookPoint<HookArgs.CheckHalloween> CheckHalloween = new HookPoint<HookArgs.CheckHalloween>();
    }
}
#endif