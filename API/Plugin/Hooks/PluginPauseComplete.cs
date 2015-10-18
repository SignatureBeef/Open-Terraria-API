#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Plugin replacing data.
        /// </summary>
        public struct PluginPauseComplete
        {
            /// <summary>
            /// The plugin being paused
            /// </summary>
            public BasePlugin Plugin { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Occurs when an OTA plugin has been paused using plugin.Pause(), and is then released.
        /// </summary>
        public static readonly HookPoint<HookArgs.PluginPauseComplete> PluginPauseComplete = new HookPoint<HookArgs.PluginPauseComplete>();
    }
}
#endif