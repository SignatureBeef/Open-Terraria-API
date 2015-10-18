#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    //SUBJECT TO CHANGE
    public static partial class HookArgs
    {
        public struct DeathMessage
        {
            public int Player { get; set; }

            public int NPC { get; set; }

            public int Projectile { get; set; }

            public int Other { get; set; }

            public Terraria.Entity Sender { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.DeathMessage> DeathMessage = new HookPoint<HookArgs.DeathMessage>();
    }
}
#endif