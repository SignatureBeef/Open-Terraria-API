#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Plugin replacing data.
        /// </summary>
        public struct PluginReplacing
        {
            /// <summary>
            /// The plugin being replaced
            /// </summary>
            public BasePlugin OldPlugin { get; set; }

            /// <summary>
            /// The new plugin taking the OldPlugins' place
            /// </summary>
            public BasePlugin NewPlugin { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Occurs when OTA is replacing the plugin with another instance.
        /// </summary>
        public static readonly HookPoint<HookArgs.PluginReplacing> PluginReplacing = new HookPoint<HookArgs.PluginReplacing>();
    }
}
#endif