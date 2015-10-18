#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Plugin replacing data.
        /// </summary>
        public struct PluginPausing
        {
            /// <summary>
            /// The plugin being paused
            /// </summary>
            public BasePlugin Plugin { get; set; }

            /// <summary>
            /// Gets or sets the signal.
            /// </summary>
            public System.Threading.ManualResetEvent Signal { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Occurs when an OTA plugin is pausing using plugin.Pause()
        /// </summary>
        public static readonly HookPoint<HookArgs.PluginPausing> PluginPausing = new HookPoint<HookArgs.PluginPausing>();
    }
}
#endif