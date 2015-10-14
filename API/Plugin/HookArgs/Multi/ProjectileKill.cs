#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct ProjectileKill
        {
            public int Index { get; set; }
            public int Id { get; set; }
            public int Owner { get; set; }
        }
    }
}
#endif