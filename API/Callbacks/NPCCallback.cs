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
            var args = new HookArgs.NpcSpawn()
            {
                X = x,
                Y = y,
                Type = type,
                Start = start
            };

            HookPoints.NpcSpawn.Invoke(ref ctx, ref args);

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
            var args = new HookArgs.InvasionNpcSpawn()
            {
                X = x,
                Y = y
            };

            HookPoints.InvasionNpcSpawn.Invoke(ref ctx, ref args);

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
            var args = new HookArgs.NpcKilled()
            {
                Type = npc.type,
                NetId = npc.netID
            };

            HookPoints.NpcKilled.Invoke(ref ctx, ref args);
        }
        
        #else
        public static void OnNPCKilled(object npc)
        {
        }
        #endif

        public static bool OnStrike(Terraria.NPC npc, ref double damage)
        {
            damage = 0;

            var ctx = new HookContext();
            var args = new HookArgs.NpcStrike()
            {
                Npc = npc,
                Damage = damage
            };

            HookPoints.NpcStrike.Invoke(ref ctx, ref args);

            damage = args.Damage;
            if (ctx.ResultParam != null && ctx.ResultParam is double)
                damage = (double)ctx.ResultParam; 

            return ctx.Result == HookResult.DEFAULT; //If default then continue onto vanillacode
        }

        #region "Creation Calls"

        public static void OnSetDefaultsBegin(Terraria.NPC npc, int type, float scaleOverride = -1)
        {
            var ctx = HookContext.Empty;
            var args = new HookArgs.NpcSetDefaultsByType()
            {
                State = MethodState.Begin,
                Type = type,
                ScaleOverride = scaleOverride,

                Npc = npc
            };

            HookPoints.NpcSetDefaultsByType.Invoke(ref ctx, ref args);
        }

        public static void OnSetDefaultsEnd(Terraria.NPC npc, int type, float scaleOverride = -1)
        {
            var ctx = HookContext.Empty;
            var args = new HookArgs.NpcSetDefaultsByType()
            {
                State = MethodState.End,
                Type = type,
                ScaleOverride = scaleOverride,

                Npc = npc
            };

            HookPoints.NpcSetDefaultsByType.Invoke(ref ctx, ref args);
        }

        public static void OnSetDefaultsBegin(Terraria.NPC npc, string name)
        {
            var ctx = HookContext.Empty;
            var args = new HookArgs.NpcSetDefaultsByName()
            {
                State = MethodState.Begin,
                Name = name,

                Npc = npc
            };

            HookPoints.NpcSetDefaultsByName.Invoke(ref ctx, ref args);
        }

        public static void OnSetDefaultsEnd(Terraria.NPC npc, string name)
        {
            var ctx = HookContext.Empty;
            var args = new HookArgs.NpcSetDefaultsByName()
            {
                State = MethodState.End,
                Name = name,

                Npc = npc
            };

            HookPoints.NpcSetDefaultsByName.Invoke(ref ctx, ref args);
        }

        public static void OnNetDefaultsBegin(Terraria.NPC npc, int type)
        {
            var ctx = HookContext.Empty;
            var args = new HookArgs.NpcNetDefaults()
            {
                State = MethodState.End,
                Type = type,

                Npc = npc
            };

            HookPoints.NpcNetDefaults.Invoke(ref ctx, ref args);
        }

        public static void OnNetDefaultsEnd(Terraria.NPC npc, int type)
        {
            var ctx = HookContext.Empty;
            var args = new HookArgs.NpcNetDefaults()
            {
                State = MethodState.End,
                Type = type,

                Npc = npc
            };

            HookPoints.NpcNetDefaults.Invoke(ref ctx, ref args);
        }

        #endregion
    }
}
