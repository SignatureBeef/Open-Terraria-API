using System;
using OTA.Plugin;
using OTA.Logging;

namespace OTA.Callbacks
{
    public static class PlayerCallback
    {
        //Death message
        //Player killed
        //Teleport
        #if Full_API
        public static bool OnPlayerHurt(Terraria.Player player, int damage, int hitDirection, bool pvp = false, bool quiet = false, string deathText = " was slain...", bool crit = false, int cooldownCounter = -1)
        {
            var ctx = new HookContext()
            {
//                Sender = player, This 
//                Player = player
            };
            var args = new HookArgs.PlayerHurt()
            {
                Victim = player,
                Damage = damage,
                HitDirection = hitDirection,
                Pvp = pvp,
                Quiet = quiet,
                Obituary = deathText,
                Critical = crit,
                CooldownCounter = cooldownCounter 
            };

            HookPoints.PlayerHurt.Invoke(ref ctx, ref args);

            if (ctx.CheckForKick()) return false;

            return ctx.Result == HookResult.DEFAULT;
        }

        public static bool OnPlayerKilled(Terraria.Player player, ref double dmg, ref int hitDirection, ref bool pvp, ref string deathText)
        {
            var ctx = new HookContext()
            {
                Sender = player,
                Player = player
            };
            var args = new HookArgs.PlayerKilled()
            {
                Damage = dmg,
                HitDirection = hitDirection,
                PvP = pvp,
                DeathText = deathText
            };

            HookPoints.PlayerKilled.Invoke(ref ctx, ref args);

            if (ctx.CheckForKick()) return false;

            if (ctx.Result == HookResult.DEFAULT)
            {
                deathText = player.name + deathText;
                return true;
            }

            dmg = args.Damage;
            hitDirection = args.HitDirection;
            pvp = args.PvP;
            deathText = args.DeathText;
            return ctx.Result != HookResult.IGNORE;
        }
        
#else
        public static bool OnPlayerHurt(object player, int damage, int hitDirection, bool pvp = false, bool quiet = false, string deathText = " was slain...", bool crit = false, int cooldownCounter = -1)
        {
            return false;
        }

        public static bool OnPlayerKilled(object player, ref double dmg, ref int hitDirection, ref bool pvp, ref string deathText)
        {
            return false;
        }
        #endif
    }
}

