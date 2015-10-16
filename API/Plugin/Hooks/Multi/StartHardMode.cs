#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct StartHardMode
        {
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.StartHardMode> StartHardMode = new HookPoint<HookArgs.StartHardMode>();
    }
}
#endif