using OTA.Plugin;
using OTA.Command;
#if CLIENT
using Microsoft.Xna.Framework.Graphics;
#endif

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

        public static Terraria.NPC OnNewNpc(int index, int x, int y, int type, int start, float ai0, float ai1, float ai2, float ai3, int target)
        {
            var ctx = new HookContext();
            var args = new HookArgs.NewNpc()
            {
                Type = type,
                NpcIndex = index,
                X = x,
                Y = y,
                Start = start,
                AI0 = ai0,
                AI1 = ai1,
                AI2 = ai2,
                AI3 = ai3,
                Target = target
            };

            HookPoints.NewNpc.Invoke(ref ctx, ref args);

            if (ctx.Result == HookResult.RECTIFY && ctx.ResultParam is Terraria.NPC) return (Terraria.NPC)ctx.ResultParam;

            var npc = new Terraria.NPC();
//            var npc = new NpcTest();
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
        #endif

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

        #if CLIENT
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

        public static void OnGetChatButtons(ref string text, ref string text2)
        {
            var ctx = new HookContext();
            var args = new HookArgs.NpcGetChatButtons()
            {
                Npc = Terraria.Main.npc[Terraria.Main.player[Terraria.Main.myPlayer].talkNPC],
                Buttons = new string[] { text, text2 }
            };

            HookPoints.NpcGetChatButtons.Invoke(ref ctx, ref args);

            if (ctx.Result == HookResult.RECTIFY)
            {
                var arr = ctx.ResultParam as string[];
                if (arr != null)
                {
                    if (arr.Length == 1)
                    {
                        text = arr[0] ?? string.Empty;
                        text2 = string.Empty;
                    }
                    else if (arr.Length == 2)
                    {
                        text = arr[0] ?? string.Empty;
                        text2 = arr[1] ?? string.Empty;
                    }
                }
            }
        }

        public static bool OnChatButtonClicked()
        {
            var res = OTA.Mod.Npc.NpcChatButton.None;

            if (Terraria.Main.npcChatFocus1)
            {
                res = OTA.Mod.Npc.NpcChatButton.Second;
            }
            else if (Terraria.Main.npcChatFocus2)
            {
                res = OTA.Mod.Npc.NpcChatButton.First;
            }
            else if (Terraria.Main.npcChatFocus3)
            {
                res = OTA.Mod.Npc.NpcChatButton.Third;
            }

            if (res != OTA.Mod.Npc.NpcChatButton.None)
            {
                var ctx = new HookContext();
                var args = new HookArgs.NpcChatButtonClick()
                {
                    Npc = Terraria.Main.npc[Terraria.Main.player[Terraria.Main.myPlayer].talkNPC],
                    Button = res
                };

                HookPoints.NpcChatButtonClick.Invoke(ref ctx, ref args);

                return ctx.Result == HookResult.DEFAULT;
            }

            return true;
        }
        #endif

        public static bool OnPreSpawn
        (
            #if CLIENT
            Microsoft.Xna.Framework.Rectangle prm0,//prm
            Microsoft.Xna.Framework.Rectangle prm1,//rectangle
            Microsoft.Xna.Framework.Vector2 prm2,//center
            Microsoft.Xna.Framework.Vector2 prm3,//center2
            Microsoft.Xna.Framework.Vector2 prm4,//center3
            Microsoft.Xna.Framework.Vector2 prm5,//bottom
            System.Boolean prm6,//flag13
            System.Boolean sky,//flag
            System.Boolean lihzahrdBrickWall,//flag2
            System.Boolean playerSafe,//flag3
            System.Boolean invasion,//flag4
            System.Boolean water,//flag5
            System.Boolean prm12,//flag6
            System.Boolean granite,//flag7
            System.Boolean marble,//flag8
            System.Boolean spiderCave,//flag9
            System.Boolean playerInTown,//flag10
            System.Boolean desertCave,//flag11
            System.Boolean planteraDefeated,//flag12
            System.Boolean safeRangeX,//flag14
            System.Int32 spawnTileX,//num
            System.Int32 spawnTileY,//num2
            System.Int32 prm22,//num3
            System.Int32 prm23,//num4
            System.Int32 prm24,//i
            System.Int32 playerId,//j
            System.Int32 prm26,//num5
            System.Int32 prm27,//k
            System.Int32 prm28,//num6
            System.Int32 prm29,//num7
            System.Int32 prm30,//num9
            System.Int32 prm31,//nunm10
            System.Int32 prm32,//num11
            System.Int32 prm33,//num12
            System.Int32 prm34,//num13
            System.Int32 prm35,//num14
            System.Int32 prm36,//num15
            System.Int32 prm37,//num16
            System.Int32 prm38,//l
            System.Int32 prm39,//num17
            System.Int32 prm40,//num18
            System.Int32 prm41,//m
            System.Int32 prm42,//num19
            System.Int32 prm43,//num20
            System.Int32 prm44,//num21
            System.Int32 prm45,//num22
            System.Int32 prm46,//n
            System.Int32 prm47,//num23
            System.Int32 prm48,//num24
            System.Int32 playerFloorX,//num25
            System.Int32 playerFloorY,//num26
            System.Int32 prm51,//num27
            System.Int32 prm52,//num28
            System.Int32 prm53,//num29
            System.Int32 prm54,//num30
            System.Int32 prm55,//num31
            System.Int32 prm56,//num32
            System.Int32 prm57,//num33
            System.Int32 prm58,//num34
            System.Int32 prm59,//num35
            System.Int32 prm60,//num36
            System.Int32 prm61,//num37
            System.Int32 prm62,//num38
            System.Int32 prm63,//num39
            System.Int32 prm64,//num40
            System.Int32 prm65,//num41
            System.Int32 prm66,//num42
            System.Int32 prm67,//num43
            System.Int32 prm68,//num44
            System.Int32 prm69,//num45
            System.Int32 prm70,//num46
            System.Single prm71//num8
            #elif SERVER
            Microsoft.Xna.Framework.Rectangle prm0/*prm*/,
            Microsoft.Xna.Framework.Rectangle prm1/*prm2*/,
            System.Boolean prm2/*flag13*/,
            System.Boolean sky/*flag*/,
            System.Boolean lihzahrdBrickWall/*flag2*/,
            System.Boolean playerSafe/*flag3*/,
            System.Boolean invasion/*flag4*/,
            System.Boolean water/*flag5*/,
            System.Boolean prm8/*flag6*/,
            System.Boolean granite/*flag7*/,
            System.Boolean marble/*flag8*/,
            System.Boolean spiderCave/*flag9*/,
            System.Boolean playerInTown/*flag10*/,
            System.Boolean desertCave/*flag11*/,
            System.Boolean planteraDefeated/*flag12*/,
            System.Boolean safeRangeX/*flag14*/,
            System.Int32 spawnTileX/*num*/,
            System.Int32 spawnTileY/*num2*/,
            System.Int32 prm18/*num3*/,
            System.Int32 prm19/*num4*/,
            System.Int32 prm20/*i*/,
            System.Int32 prm21/*j*/,
            System.Int32 prm22/*num5*/,
            System.Int32 prm23/*k*/,
            System.Int32 prm24/*num6*/,
            System.Int32 prm25/*num7*/,
            System.Int32 prm26/*num9*/,
            System.Int32 prm27/*num10*/,
            System.Int32 prm28/*num11*/,
            System.Int32 prm29/*num12*/,
            System.Int32 prm30/*num13*/,
            System.Int32 prm31/*num14*/,
            System.Int32 prm32/*num15*/,
            System.Int32 prm33/*num16*/,
            System.Int32 prm34/*l*/,
            System.Int32 prm35/*num17*/,
            System.Int32 prm36/*num18*/,
            System.Int32 prm37/*m*/,
            System.Int32 prm38/*num19*/,
            System.Int32 prm39/*num20*/,
            System.Int32 prm40/*num21*/,
            System.Int32 prm41/*num22*/,
            System.Int32 prm42/*n*/,
            System.Int32 prm43/*num23*/,
            System.Int32 prm44/*num24*/,
            System.Int32 playerFloorX/*num25*/,
            System.Int32 playerFloorY/*num26*/,
            System.Int32 prm47/*num27*/,
            System.Int32 prm48/*num28*/,
            System.Int32 prm49/*num29*/,
            System.Int32 prm50/*num30*/,
            System.Int32 prm51/*num31*/,
            System.Int32 prm52/*num32*/,
            System.Int32 prm53/*num33*/,
            System.Int32 prm54/*num34*/,
            System.Int32 prm55/*num35*/,
            System.Int32 prm56/*num36*/,
            System.Int32 prm57/*num37*/,
            System.Int32 prm58/*num38*/,
            System.Int32 prm59/*num39*/,
            System.Int32 prm60/*num40*/,
            System.Int32 prm61/*num41*/,
            System.Int32 prm62/*num42*/,
            System.Int32 prm63/*num43*/,
            System.Int32 prm64/*num44*/,
            System.Int32 prm65/*num45*/,
            System.Int32 prm66/*num46*/,
            System.Single prm67/*num8*/
            #endif
        )
        {
            var ctx = new HookContext();
            var args = new HookArgs.NpcPreSpawn()
            {
                Sky = sky,
                LihzahrdBrickWall = lihzahrdBrickWall,
                PlayerSafe = playerSafe,
                Invasion = invasion,
                Water = water,
                Granite = granite,
                Marble = marble,
                SpiderCave = spiderCave,
                PlayerInTown = playerInTown,
                DesertCave = desertCave,
                PlanteraDefeated = planteraDefeated,
                SafeRangeX = safeRangeX,
                SpawnTileX = spawnTileX,
                SpawnTileY = spawnTileY,
                PlayerFloorX = playerFloorX,
                PlayerFloorY = playerFloorY
            };

            HookPoints.NpcPreSpawn.Invoke(ref ctx, ref args);

            return ctx.Result == HookResult.DEFAULT;
        }

//        public static int DropLoot(int X, int Y, int Width, int Height, int Type, int Stack, bool noBroadcast, int pfix, bool noGrabDelay, bool reverseLookup, int npc)
//        {
//            return 0;//return Terraria.Main.NPCLoo((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, 1533, 1, false, 0, false, false);
        //        }

//        #if CLIENT
//        static readonly object _textureLock = new object();
//
//        public static Texture2D GetTexture(int index)
//        {
//            lock (_textureLock)
//                return Terraria.Main.npcTexture[index];
//        }
//
//        public static void SetTexture(int index, Texture2D texture)
//        {
//            lock (_textureLock)
//                Terraria.Main.npcTexture[index] = texture;
//        }
//        #endif
    }
}
