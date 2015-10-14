#if CLIENT
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct UpdateClient { public MethodState State { get; set; } }
    }
}
#endif