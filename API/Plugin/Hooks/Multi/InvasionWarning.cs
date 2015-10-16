#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct InvasionWarning
        {
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.InvasionWarning> InvasionWarning = new HookPoint<HookArgs.InvasionWarning>();
    }
}
#endif