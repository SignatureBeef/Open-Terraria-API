#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct WorldSave
        {
            public MethodState State { get; set; }

            public bool ResetTime { get; set; }

            public bool UseCloudSaving { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.WorldSave> WorldSave = new HookPoint<HookArgs.WorldSave>();
    }
}
#endif