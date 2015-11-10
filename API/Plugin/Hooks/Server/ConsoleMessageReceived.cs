#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Console message data.
        /// </summary>
        public struct ConsoleMessageReceived
        {
            /// <summary>
            /// The message to be written to the console
            /// </summary>
            public string Message { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Occurs when console text is written.
        /// </summary>
        /// <remarks>This can typically be used to populate external consoles.</remarks>
        /// <description>This hook cannot be cancelled</description>
        public static readonly HookPoint<HookArgs.ConsoleMessageReceived> ConsoleMessageReceived = new HookPoint<HookArgs.ConsoleMessageReceived>();
    }
}
#endif