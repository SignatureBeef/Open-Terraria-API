#if CLIENT
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct Update { public MethodState State { get; set; } }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.Update> Update = HookPoint<HookArgs.Update>();
    }
}
#endif