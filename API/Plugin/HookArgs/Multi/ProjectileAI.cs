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
}
#endif