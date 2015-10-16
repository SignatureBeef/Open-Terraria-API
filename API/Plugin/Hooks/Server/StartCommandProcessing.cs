#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct StartCommandProcessing
        {
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.StartCommandProcessing> StartCommandProcessing = new HookPoint<HookArgs.StartCommandProcessing>();
    }
}
#endif