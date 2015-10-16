#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct PlayerKilled
        {
            public double Damage { get; set; }

            public int HitDirection { get; set; }

            public bool PvP { get; set; }

            public string DeathText { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.PlayerKilled> PlayerKilled = new HookPoint<HookArgs.PlayerKilled>();
    }
}
#endif