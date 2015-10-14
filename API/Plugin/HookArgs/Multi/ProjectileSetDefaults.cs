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
}
#endif