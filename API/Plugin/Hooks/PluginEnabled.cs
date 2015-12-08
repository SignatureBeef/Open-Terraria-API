#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Plugin enable data.
        /// </summary>
        public struct PluginEnabled
        {
            /// <summary>
            /// The plugin that was enabled
            /// </summary>
            public BasePlugin Plugin { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Occurs when an OTA plugin has enabled
        /// </summary>
        public static readonly HookPoint<HookArgs.PluginEnabled> PluginEnabled = new HookPoint<HookArgs.PluginEnabled>();
    }
}
#endif