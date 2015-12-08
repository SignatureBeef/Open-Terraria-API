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
            var ctx = new HookContext();
            var args = new HookArgs.NpcKilled()
            {
                Type = npc.type,
                NetId = npc.netID,
                Npc = npc
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

            return ctx.Result == HookResult.DEFAULT; //If default then continue on to vanillacode
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

        public static bool OnTransformBegin(Terraria.NPC npc, int newType)
        {
            var ctx = new HookContext();
            var args = new HookArgs.NpcTransform()
            {
                State = MethodState.Begin,
                NewType = newType,

                Npc = npc
            };

            HookPoints.NpcTransform.Invoke(ref ctx, ref args);

            return ctx.Result == HookResult.DEFAULT;
        }

        public static void OnTransformEnd(Terraria.NPC npc, int newType)
        {
            var ctx = new HookContext();
            var args = new HookArgs.NpcTransform()
            {
                State = MethodState.End,
                NewType = newType,

                Npc = npc
            };

            HookPoints.NpcTransform.Invoke(ref ctx, ref args);
        }

        public static bool OnDropLootBegin(ref int itemId, int x, int y, int width, int height, int type, int stack = 1, bool noBroadcast = false, int prefix = 0, bool noGrabDelay = false, bool reverseLookup = false)
        {
            itemId = 0;

            var ctx = new HookContext();
            var args = new HookArgs.NpcDropLoot()
            {
                State = MethodState.Begin,

                X = x,
                Y = y,
                Width = width,
                Height = height,
                Type = type,
                Stack = stack,
                NoBroadcast = noBroadcast,
                Prefix = prefix,
                NoGrabDelay = noGrabDelay,
                ReverseLookup = reverseLookup
            };

            HookPoints.NpcDropLoot.Invoke(ref ctx, ref args);

            if (ctx.ResultParam != null && ctx.ResultParam is int)
                itemId = (int)ctx.ResultParam;
            else if (ctx.Result == HookResult.IGNORE)
                itemId = -1;

            return ctx.Result == HookResult.DEFAULT; //If default then continue on to vanillacode
        }

        public static void OnDropLootEnd(int x, int y, int width, int height, int type, int stack = 1, bool noBroadcast = false, int prefix = 0, bool noGrabDelay = false, bool reverseLookup = false)
        {
            var ctx = new HookContext();
            var args = new HookArgs.NpcDropLoot()
            {
                State = MethodState.End,

                X = x,
                Y = y,
                Width = width,
                Height = height,
                Type = type,
                Stack = stack,
                NoBroadcast = noBroadcast,
                Prefix = prefix,
                NoGrabDelay = noGrabDelay,
                ReverseLookup = reverseLookup
            };

            HookPoints.NpcDropLoot.Invoke(ref ctx, ref args);
        }

        public static bool OnDropBossBagBegin(ref int itemId, int x, int y, int width, int height, int type, int stack = 1, bool noBroadcast = false, int prefix = 0, bool noGrabDelay = false, bool reverseLookup = false)
        {
            itemId = 0;

            var ctx = new HookContext();
            var args = new HookArgs.NpcDropBossBag()
            {
                State = MethodState.Begin,

                X = x,
                Y = y,
                Width = width,
                Height = height,
                Type = type,
                Stack = stack,
                NoBroadcast = noBroadcast,
                Prefix = prefix,
                NoGrabDelay = noGrabDelay,
                ReverseLookup = reverseLookup
            };

            HookPoints.NpcDropBossBag.Invoke(ref ctx, ref args);

            if (ctx.ResultParam != null && ctx.ResultParam is int)
                itemId = (int)ctx.ResultParam;
            else if (ctx.Result == HookResult.IGNORE)
                itemId = -1;

            return ctx.Result == HookResult.DEFAULT; //If default then continue on to vanillacode
        }

        public static void OnDropBossBagEnd(int x, int y, int width, int height, int type, int stack = 1, bool noBroadcast = false, int prefix = 0, bool noGrabDelay = false, bool reverseLookup = false)
        {
            var ctx = new HookContext();
            var args = new HookArgs.NpcDropBossBag()
            {
                State = MethodState.End,

                X = x,
                Y = y,
                Width = width,
                Height = height,
                Type = type,
                Stack = stack,
                NoBroadcast = noBroadcast,
                Prefix = prefix,
                NoGrabDelay = noGrabDelay,
                ReverseLookup = reverseLookup
            };

            HookPoints.NpcDropBossBag.Invoke(ref ctx, ref args);
        }

        //OnSpawnInvasionNPC

        public static int OnSpawnInvasionNPC(int x, int y, int type, int start, float ai0, float ai1, float ai2, float ai3, int target)
        {
            var ctx = new HookContext();
            var args = new HookArgs.InvasionNpcSpawn()
            {
                State = MethodState.Begin,

                X = x,
                Y = y,
                Type = type,
                Start = start,
                AI0 = ai0,
                AI1 = ai1,
                AI2 = ai2,
                AI3 = ai3,
                Target = target
            };

            HookPoints.InvasionNpcSpawn.Invoke(ref ctx, ref args);

            if (ctx.Result == HookResult.IGNORE) return 0;
            else if (ctx.Result == HookResult.RECTIFY && ctx.ResultParam is int) return (int)ctx.ResultParam;

            args.Start = Terraria.NPC.NewNPC(args.X, args.Y, args.Type, args.Start, args.AI0, args.AI1, args.AI2, args.AI3, args.Target);
            args.State = MethodState.End;

            HookPoints.InvasionNpcSpawn.Invoke(ref ctx, ref args);

            return args.Start;
        }

        public static Terraria.NPC OnNewNpc(int type)
        {
            var ctx = new HookContext();
            var args = new HookArgs.NewNpc()
            {
                Type = type
            };

            HookPoints.NewNpc.Invoke(ref ctx, ref args);

            if (ctx.Result == HookResult.RECTIFY && ctx.ResultParam is Terraria.NPC) return (Terraria.NPC)ctx.ResultParam;

            var npc = new Terraria.NPC();
            npc.SetDefaults(type, -1);
            return npc;
        }

        #if CLIENT
        public static bool OnDrawNPCBegin(Terraria.Main game, int i, bool behindTiles)
        {
            var ctx = new HookContext();
            var args = new HookArgs.NpcDraw()
            {
                State = MethodState.Begin,

                Npc = Terraria.Main.npc[i],
                BehindTiles = behindTiles
            };

            HookPoints.NpcDraw.Invoke(ref ctx, ref args);

            return ctx.Result == HookResult.DEFAULT;
        }

        public static void OnDrawNPCEnd(Terraria.Main game, int i, bool behindTiles)
        {
            var ctx = new HookContext();
            var args = new HookArgs.NpcDraw()
            {
                State = MethodState.End,

                Npc = Terraria.Main.npc[i],
                BehindTiles = behindTiles
            };

            HookPoints.NpcDraw.Invoke(ref ctx, ref args);
        }

        public static bool OnUpdateNPCBegin(Terraria.NPC npc, int i)
        {
            var ctx = new HookContext();
            var args = new HookArgs.NpcUpdate()
            {
                State = MethodState.Begin,

                Npc = Terraria.Main.npc[i],
                NpcIndex = i
            };

            HookPoints.NpcUpdate.Invoke(ref ctx, ref args);

            return ctx.Result == HookResult.DEFAULT;
        }

        public static void OnUpdateNPCEnd(Terraria.NPC npc, int i)
        {
            var ctx = new HookContext();
            var args = new HookArgs.NpcUpdate()
            {
                State = MethodState.End,

                Npc = Terraria.Main.npc[i],
                NpcIndex = i
            };

            HookPoints.NpcUpdate.Invoke(ref ctx, ref args);
        }

        public static bool OnAIBegin(Terraria.NPC npc)
        {
            var ctx = new HookContext();
            var args = new HookArgs.NpcAI()
            {
                State = MethodState.Begin,

                Npc = npc
            };

            HookPoints.NpcAI.Invoke(ref ctx, ref args);

            return ctx.Result == HookResult.DEFAULT;
        }

        public static void OnAIEnd(Terraria.NPC npc)
        {
            var ctx = new HookContext();
            var args = new HookArgs.NpcAI()
            {
                State = MethodState.End,

                Npc = npc
            };

            HookPoints.NpcAI.Invoke(ref ctx, ref args);
        }

        public static bool OnFindFrameBegin(Terraria.NPC npc)
        {
            var ctx = new HookContext();
            var args = new HookArgs.NpcFindFrame()
            {
                State = MethodState.Begin,

                Npc = npc
            };

            HookPoints.NpcFindFrame.Invoke(ref ctx, ref args);

            return ctx.Result == HookResult.DEFAULT;
        }

        public static void OnFindFrameEnd(Terraria.NPC npc)
        {
            var ctx = new HookContext();
            var args = new HookArgs.NpcFindFrame()
            {
                State = MethodState.End,

                Npc = npc
            };

            HookPoints.NpcFindFrame.Invoke(ref ctx, ref args);
        }

        public static string OnGetChat(Terraria.NPC npc)
        {
            var ctx = new HookContext();
            var args = new HookArgs.NpcGetChat()
            {
                Npc = npc
            };

            HookPoints.NpcGetChat.Invoke(ref ctx, ref args);

            return (ctx.ResultParam as string) ?? npc.GetChatDirect();
        }
        #endif
    }
}
