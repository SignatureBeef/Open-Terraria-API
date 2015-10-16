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

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.ProjectileKill> ProjectileKill = new HookPoint<HookArgs.ProjectileKill>();
    }
}
#endif