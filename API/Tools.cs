using System;
using System.Linq;
using Microsoft.Xna.Framework;
using OTA.Logging;

#if Full_API
using Terraria;
using System.Collections.Generic;
#endif

namespace OTA
{
    /// <summary>
    /// General utilities for plugins to use
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// Notifies all players.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="color">Color.</param>
        /// <param name="writeToConsole">If set to <c>true</c> write to console.</param>
        public static int NotifyAllPlayers(string message, Color color, bool writeToConsole = true) //, SendingLogger Logger = SendingLogger.CONSOLE)
        {
            var sentTo = 0;
#if Full_API
            foreach (var player in Main.player)
            {
                if (player != null && player.active)
                {
                    NetMessage.SendData((int)Packet.PLAYER_CHAT, player.whoAmI, -1, message, 255 /* PlayerId */, color.R, color.G, color.B);
                    sentTo++;
                }
            }

            if (writeToConsole)
                ProgramLog.Log(message);
#endif

            return sentTo;
        }

        #if Full_API
        /// <summary>
        /// Gets a specified Online Player
        /// Input name must already be cleaned of spaces
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Player GetPlayerByName(string name)
        {
            string lowercaseName = name.ToLower();
            foreach (Player player in Main.player)
            {
                if (player != null && player.active && player.name.ToLower().Equals(lowercaseName))
                    return player;
            }
            return null;
        }

        /// <summary>
        /// Gets the total of all active NPCs
        /// </summary>
        /// <returns></returns>
        public static int ActiveNPCCount()
        {
            return (from x in Main.npc
                             where x != null && x.active
                             select x).Count();
        }

        /// <summary>
        /// Finds a valid location for such things as NPC Spawning
        /// </summary>
        /// <param name="point"></param>
        /// <param name="defaultResist"></param>
        /// <returns></returns>
        public static bool IsValidLocation(Vector2 point, bool defaultResist = true)
        {
            if ((defaultResist) ? (point != Vector2.Zero) : true)
            if (point.X <= Main.maxTilesX && point.X >= 0)
            {
                if (point.Y <= Main.maxTilesY && point.Y >= 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks online players for a matching name part
        /// </summary>
        /// <param name="partName"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static List<Player> FindPlayerByPart(string partName, bool ignoreCase = true)
        {
            List<Player> matches = new List<Player>();

            foreach (var player in Main.player)
            {
                if (player == null || player.name == null)
                    continue;

                string playerName = player.name;

                if (ignoreCase)
                    playerName = playerName.ToLower();

                if (playerName.StartsWith((ignoreCase) ? partName.ToLower() : partName))
                    matches.Add(player);
            }

            return matches;
        }

        /// <summary>
        /// Used to clean the names of Items or NPC's for parsing
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CleanName(string input)
        {
            return input.Replace(" ", String.Empty).ToLower();
        }

        /// <summary>
        /// Attempts to find the first online player
        ///		Usually the Slot Manager assigns them to the lowest possible index
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static bool TryGetFirstOnlinePlayer(out Player player)
        {
            player = null;
            try
            {
                for (var i = 0; i < Main.player.Length; i++)
                {
                    var ply = Main.player[i];
                    if (ply != null && ply.active && ply.name.Trim() != String.Empty)
                    {
                        player = ply;
                        return true;
                    }
                }
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// Checks if there are any active NPCs of specified type
        /// </summary>
        /// <param name="type">TypeId of NPC to check for</param>
        /// <returns>True if active, false if not</returns>
        public static bool IsNPCSummoned(int type) => NPC.AnyNPCs(type);

        /// <summary>
        /// Checks if there are any active NPCs of specified name
        /// </summary>
        /// <param name="Name">Name of NPC to check for</param>
        /// <returns>True if active, false if not</returns>
        public static bool IsNPCSummoned(string name)
        {
            int id;
            return TryFindNPCByName(name, out id);
        }

        /// <summary>
        /// Tries to find the first NPC by name
        /// </summary>
        /// <returns><c>true</c>, if find NPC by name was tryed, <c>false</c> otherwise.</returns>
        /// <param name="name">Name.</param>
        /// <param name="id">Identifier.</param>
        public static bool TryFindNPCByName(string name, out int id)
        {
            id = default(Int32);

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];
                if (npc != null && npc.active && npc.name == name)
                {
                    id = npc.whoAmI;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Finds the existing projectile a player.
        /// </summary>
        /// <returns>The existing projectile for user.</returns>
        /// <param name="playerId">Player identifier.</param>
        /// <param name="identity">Identity.</param>
        public static int FindExistingProjectileForPlayer(int playerId, int identity)
        {
            for (int x = 0; x < 1000; x++)
            {
                var prj = Main.projectile[x];
                if (prj != null && prj.owner == playerId && prj.identity == identity && prj.active)
                    return x;
            }
            return -1;
        }

        /// <summary>
        /// Gets the available NPC slots count.
        /// </summary>
        /// <value>The available NPC slots.</value>
        public static int AvailableNPCSlots
        {
            get
            {
                return Main.npc
                  .Where(x => x == null || !x.active)
                  .Count();
            }
        }

        /// <summary>
        /// Gets the used NPC slots count.
        /// </summary>
        /// <value>The used NPC slots.</value>
        public static int UsedNPCSlots
        {
            get
            {
                return Main.npc
                  .Where(x => x != null && x.active)
                  .Count();
            }
        }

        /// <summary>
        /// Gets the available item slots count.
        /// </summary>
        /// <value>The available item slots.</value>
        public static int AvailableItemSlots
        {
            get
            {
                return Main.item
                  .Where(x => x == null || !x.active)
                  .Count();
            }
        }

        /// <summary>
        /// Gets the used item slots count.
        /// </summary>
        /// <value>The used item slots.</value>
        public static int UsedItemSlots
        {
            get
            {
                return Main.item
                  .Where(x => x != null && x.active)
                  .Count();
            }
        }

        /// <summary>
        /// Gets the active player count.
        /// </summary>
        /// <value>The active player count.</value>
        public static int ActivePlayerCount
        {
            get
            {
                return (from p in Terraria.Main.player
                                    where p != null && p.active
                                    select p.name).Count();
            }
        }

        /// <summary>
        /// Gets the max players.
        /// </summary>
        /// <value>The max players.</value>
        public static int MaxPlayers { get; } = Main.maxNetPlayers;
        #endif

        /// <summary>
        /// Gets the runtime platform.
        /// </summary>
        /// <value>The runtime platform.</value>
        public static Misc.RuntimePlatform RuntimePlatform
        {
            get
            { return Type.GetType("Mono.Runtime") != null ? Misc.RuntimePlatform.Mono : Misc.RuntimePlatform.Microsoft; }
        }
    }
}