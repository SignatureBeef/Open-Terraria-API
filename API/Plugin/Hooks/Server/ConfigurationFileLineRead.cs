#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Configuration file line data.
        /// </summary>
        public struct ConfigurationFileLineRead
        {
            /// <summary>
            /// The key of the line
            /// </summary>
            /// <remarks>Taken from the line format: key=value</remarks>
            public string Key { get; set; }

            /// <summary>
            /// The value of the line
            /// </summary>
            /// <remarks>Taken from the line format: key=value</remarks>
            public string Value { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Called when a line in the configuration file is read, but not parsed by vanilla code.
        /// </summary>
        /// <description>This hook cannot be cancelled</description>
        public static readonly HookPoint<HookArgs.ConfigurationFileLineRead> ConfigurationFileLineRead = new HookPoint<HookArgs.ConfigurationFileLineRead>();
    }
}
#endif