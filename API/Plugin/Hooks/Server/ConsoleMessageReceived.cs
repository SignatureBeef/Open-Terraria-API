#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct ConsoleMessageReceived
        {
            public string Message { get; set; }

            public OTA.Logging.SendingLogger Logger { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.ConsoleMessageReceived> ConsoleMessageReceived = new HookPoint<HookArgs.ConsoleMessageReceived>();
    }
}
#endif