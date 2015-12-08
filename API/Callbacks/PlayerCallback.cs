using System;
using OTA.Plugin;
using OTA.Logging;

namespace OTA.Callbacks
{
    /// <summary>
    /// Calls from vanilla code in Terraria.Player
    /// </summary>
    public static class PlayerCallback
    {
        #if Full_API
        /// <summary>
        /// Callde via vanilla code when a player has been hurt
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="damage">Damage.</param>
        /// <param name="hitDirection">Hit direction.</param>
        /// <param name="pvp">If set to <c>true</c> pvp.</param>
        /// <param name="quiet">If set to <c>true</c> quiet.</param>
        /// <param name="deathText">Death text.</param>
        /// <param name="crit">If set to <c>true</c> crit.</param>
        /// <param name="cooldownCounter">Cooldown counter.</param>
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

        /// <summary>
        /// Raised by vanilla code when a player has been killed
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="dmg">Dmg.</param>
        /// <param name="hitDirection">Hit direction.</param>
        /// <param name="pvp">Pvp.</param>
        /// <param name="deathText">Death text.</param>
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

        #if SERVER
        public static bool OnNameCollision(Terraria.Player connectee, int bufferId)
        {
            var ctx = new HookContext();
            var args = new HookArgs.NameConflict()
            {
                Connectee = connectee,
                BufferId = bufferId
            };

            HookPoints.NameConflict.Invoke(ref ctx, ref args);

            if (ctx.CheckForKick()) return false;

            return ctx.Result == HookResult.DEFAULT; //Continue on to kicking
        }
        #endif

        #if CLIENT
        public static void OnClientEnterWorld(Terraria.Player player)
        {
            var ctx = new HookContext()
                {
                    Player = player,
                    Sender = player
                };
            var args = new HookArgs.PlayerEnteredGame
                {
                    Slot = player.whoAmI
                };

            ctx.SetResult(HookResult.DEFAULT, false);
            HookPoints.PlayerEnteredGame.Invoke(ref ctx, ref args);
            ctx.CheckForKick();
        }
        #endif
    }
}

