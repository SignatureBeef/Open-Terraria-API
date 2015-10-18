#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Plugin replacing data.
        /// </summary>
        public struct PluginDisposed
        {
            /// <summary>
            /// The plugin being replaced
            /// </summary>
            public BasePlugin Plugin { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Occurs when an OTA plugin is disposed
        /// </summary>
        public static readonly HookPoint<HookArgs.PluginDisposed> PluginDisposed = new HookPoint<HookArgs.PluginDisposed>();
    }
}
#endif