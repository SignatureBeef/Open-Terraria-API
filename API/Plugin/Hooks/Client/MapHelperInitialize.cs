#if CLIENT
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct MapHelperInitialize
        {
            public MethodState State { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.MapHelperInitialize> MapHelperInitialize = new HookPoint<HookArgs.MapHelperInitialize>();
    }
}
#endif