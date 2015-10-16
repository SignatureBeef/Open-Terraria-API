#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct PlayerHurt
        {
            #if Full_API
            public Terraria.Player Victim { get; internal set; }
            #endif
            public int Damage { get; set; }

            public int HitDirection { get; set; }

            public bool Pvp { get; set; }

            public bool Quiet { get; set; }

            public string Obituary { get; set; }

            public bool Critical { get; set; }

            public int CooldownCounter { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.PlayerHurt> PlayerHurt = new HookPoint<HookArgs.PlayerHurt>();
    }
}
#endif