#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Load configuration file event args.
        /// </summary>
        public struct LoadConfigurationFile
        {
            /// <summary>
            /// Gets or sets the file about to be loaded.
            /// </summary>
            public string File { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Occurs when the client/server is about to start reading a configuration file.
        /// </summary>
        public static readonly HookPoint<HookArgs.LoadConfigurationFile> LoadConfigurationFile = new HookPoint<HookArgs.LoadConfigurationFile>();
    }
}
#endif