using OTA.Plugin;
using OTA.Command;

namespace OTA.Callbacks
{
    /// <summary>
    /// Callbacks from Terraria.NPC
    /// </summary>
    public static class NPCCallback
    {
        public static Rand CheckedRand = new Rand();

        private static int _invasionTypeCounter = 20;

        /// <summary>
        /// Returns a new invasion type that is dedicated for the callee
        /// </summary>
        /// <returns></returns>
        public static int AssignInvasionType()
        {
            return System.Threading.Interlocked.Increment(ref _invasionTypeCounter);
        }

        /// <summary>
        /// Determines if can spawn NPC at the specified x y type start.
        /// </summary>
        /// <remarks>Called as part of Terraria.NPC.NewNPC</remarks>
        /// <returns><c>true</c> if can spawn NP the specified x y type start; otherwise, <c>false</c>.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="type">Type.</param>
        /// <param name="start">Start.</param>
        public static bool CanSpawnNPC(int x, int y, int type, int start = 0)
        {
            var ctx = new HookContext();
            var args = new HookArgs.NPCSpawn()
            {
                X = x,
                Y = y,
                Type = type,
                Start = start
            };

            HookPoints.NPCSpawn.Invoke(ref ctx, ref args);

            return ctx.Result == HookResult.DEFAULT;
        }

        /// <summary>
        /// Called when vanilla code successfully summons an Invasion NPC
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public static void OnInvasionNPCSpawn(int x, int y)
        {
            var ctx = new HookContext();
            var args = new HookArgs.InvasionNPCSpawn()
            {
                X = x,
                Y = y
            };

            HookPoints.InvasionNPCSpawn.Invoke(ref ctx, ref args);

//            return ctx.Result == HookResult.DEFAULT;
        }

        #if Full_API
        /// <summary>
        /// Called by vanilla code when a NPC has been killed
        /// </summary>
        /// <param name="npc">Npc.</param>
        public static void OnNPCKilled(Terraria.NPC npc)
        {
            var ctx = new HookContext()
            {
                Sender = npc
            };
            var args = new HookArgs.NPCKilled()
            {
                Type = npc.type,
                NetId = npc.netID
            };

            HookPoints.NPCKilled.Invoke(ref ctx, ref args);
        }
        
        #else
        public static void OnNPCKilled(object npc)
        {
        }
        #endif
    }
}
