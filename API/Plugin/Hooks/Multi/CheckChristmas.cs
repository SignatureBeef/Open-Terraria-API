#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct CheckChristmas
        {
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.CheckChristmas> CheckChristmas = new HookPoint<HookArgs.CheckChristmas>();
    }
}
#endif