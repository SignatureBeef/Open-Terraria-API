#if CLIENT
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct Update { public MethodState State { get; set; } }
    }
}
#endif