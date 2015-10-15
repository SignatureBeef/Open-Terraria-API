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
}
#endif