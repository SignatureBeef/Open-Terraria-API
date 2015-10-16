#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct ParseCommandLineArguments
        {
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.ParseCommandLineArguments> ParseCommandLineArguments = new HookPoint<HookArgs.ParseCommandLineArguments>();
    }
}
#endif