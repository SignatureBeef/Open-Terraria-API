#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct ProjectileSetDefaults
        {
            public MethodState State { get; set; }

            public Terraria.Projectile Projectile { get; set; }

            public int Type { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.ProjectileSetDefaults> ProjectileSetDefaults = new HookPoint<HookArgs.ProjectileSetDefaults>();
    }
}
#endif