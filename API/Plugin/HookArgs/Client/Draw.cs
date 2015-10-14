#if CLIENT
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct Draw { public MethodState State { get; set; } }
    }
}
#endif