#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Parse command line arguments data.
        /// </summary>
        public struct ParseCommandLineArguments
        {
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Parse unknown command line argument request.
        /// </summary>
        /// <description>This cannot be cancelled</description>
        public static readonly HookPoint<HookArgs.ParseCommandLineArguments> ParseCommandLineArguments = new HookPoint<HookArgs.ParseCommandLineArguments>();
    }
}
#endif