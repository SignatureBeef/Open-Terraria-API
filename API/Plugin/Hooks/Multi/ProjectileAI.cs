#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct ProjectileAI
        {
            public MethodState State { get; set; }

            public Terraria.Projectile Projectile { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.ProjectileAI> ProjectileAI = new HookPoint<HookArgs.ProjectileAI>();
    }
}
#endif