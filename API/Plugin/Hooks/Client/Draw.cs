#if CLIENT
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct Draw { public MethodState State { get; set; } }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.Draw> Draw = new HookPoint<HookArgs.Draw>();
    }
}
#endif