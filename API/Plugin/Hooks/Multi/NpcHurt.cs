#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct NpcHurt
        {
            #if Full_API
            public Terraria.NPC Victim { get; set; }
            #endif
            public int Damage { get; set; }

            public int HitDirection { get; set; }

            public float Knockback { get; set; }

            public bool Critical { get; set; }

            public bool FromNet { get; set; }

            public bool NoEffect { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.NpcHurt> NpcHurt = new HookPoint<HookArgs.NpcHurt>();
    }
}
#endif