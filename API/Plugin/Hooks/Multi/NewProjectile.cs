#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Data for then Terraria.Projectile.NewProjectile is called.
        /// </summary>
        public struct NewProjectile
        {
            public int Index { get; set; }

            public float X { get; set; }

            public float Y { get; set; }

            public float SpeedX { get; set; }

            public float SpeedY { get; set; }

            public int Type { get; set; }

            public int Damage { get; set; }

            public float KnockBack { get; set; }

            public int Owner { get; set; }

            public float ai0 { get; set; }

            public float ai1 { get; set; }

            public Terraria.Projectile Projectile{ get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Triggered before vanilla runs the halloween checks.
        /// </summary>
        /// <description>
        /// To prevent vanilla touching the halloween flag, set HookContext.Result to anything but HookResult.DEFAULT
        /// </description>
        public static readonly HookPoint<HookArgs.NewProjectile> NewProjectile = new HookPoint<HookArgs.NewProjectile>();
    }
}
#endif