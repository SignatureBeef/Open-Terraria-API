#if CLIENT
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct UpdateClient { public MethodState State { get; set; } }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.UpdateClient> UpdateClient = new HookPoint<HookArgs.UpdateClient>();
    }
}
#endif