#if CLIENT
using System;
using OTA.Plugin;
using OTA.Client.Npc;

namespace OTA.Client
{
    [OTAVersion(1, 0)]
    public class ClientEventManager : BasePlugin
    {
        public ClientEventManager()
        {
            this.Author = "DeathCradle";
            this.Description = "OTA Client Mod Layer";
            this.Version = "1-dev";
            this.Name = "OTA Client Layer";
        }

        protected override void Enabled()
        {
            base.Enabled();

            OTANpc.ResizeNPCArrays();
            ScanExistingPlugins();
            EntityRegistrar.ScanAssembly(typeof(ClientEventManager).Assembly);
        }

        void ScanExistingPlugins()
        {
            foreach (var plg in PluginManager.EnumeratePlugins)
            {
                if (null != plg.Assembly)
                    EntityRegistrar.ScanAssembly(plg.Assembly);
            }
        }

        [Hook]
        void OnPluginLoad(ref HookContext ctx, ref Plugin.HookArgs.PluginEnabled args)
        {
            if (null != args.Plugin.Assembly)
                EntityRegistrar.ScanAssembly(args.Plugin.Assembly);
        }

        [Hook]
        void OnNewNpc(ref HookContext ctx, ref Plugin.HookArgs.NewNpc args)
        {
            var npc = EntityRegistrar.Npcs.Create(args.Type);
            if (npc != null)
            {
                npc.type = args.Type;
                npc.OnSetDefaults();
                npc.type = args.Type;
                ctx.SetResult(HookResult.RECTIFY, true, npc);
            }
        }

        [Hook]
        void OnNpcDraw(ref HookContext ctx, ref Plugin.HookArgs.NpcDraw args)
        {
            Terraria.Main.ignoreErrors = false;
            var ota = args.Npc as OTANpc;
            if (ota != null)
            {
                if (args.State == MethodState.Begin)
                {
                    if (!ota.OnDraw(args.BehindTiles))
                    {
                        ctx.SetResult(HookResult.IGNORE);
                    }
                }
                else
                {
                    ota.OnAfterDraw(args.BehindTiles);
                }
            }
        }

        [Hook]
        void OnNpcUpdate(ref HookContext ctx, ref Plugin.HookArgs.NpcUpdate args)
        {
            var ota = args.Npc as OTANpc;
            if (ota != null)
            {
                ota.whoAmI = args.NpcIndex;

                if (args.State == MethodState.Begin)
                {
                    if (!ota.OnUpdate())
                    {
                        ctx.SetResult(HookResult.IGNORE);
                    }
                }
                else
                {
                    ota.OnAfterUpdate();
                }
            }
        }

        [Hook]
        void OnNpcAI(ref HookContext ctx, ref Plugin.HookArgs.NpcAI args)
        {
            var ota = args.Npc as OTANpc;
            if (ota != null)
            {
                if (args.State == MethodState.Begin)
                {
                    if (!ota.OnAI())
                    {
                        ctx.SetResult(HookResult.IGNORE);
                    }
                }
                else
                {
                    ota.OnAfterAI();
                }
            }
        }

        [Hook]
        void OnPlayerEnter(ref HookContext ctx, ref Plugin.HookArgs.PlayerEnteredGame args)
        {
            Terraria.NPC.NewNPC((int)(ctx.Player.position.X), (int)(ctx.Player.position.Y), EntityRegistrar.Npcs["Sassy"]);
        }

        [Hook]
        void OnNpcChat(ref HookContext ctx, ref Plugin.HookArgs.NpcGetChat args)
        {
            var ota = args.Npc as OTANpc;
            if (ota != null)
            {
                ctx.SetResult(HookResult.IGNORE, true, ota.OnChat());
            }
        }
        
        //            static int testId;
        //
        //            [Hook]
        //            void OnUpdate(ref HookContext ctx, ref Plugin.HookArgs.GameUpdate args)
        //            {
        //                if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Q))
        //                {
        //                    var npc = Terraria.Main.npc[testId];
        //                    if (npc.type != 542)
        //                    {
        //                        Logging.ProgramLog.Debug.Log("NPC type is now 542");
        //                        npc.type = 542;
        //                        Logging.ProgramLog.Log("Main.npcFrameCount: " + Main.npcFrameCount[npc.type]);
        //                    }
        //                }
        //                else if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
        //                {
        //                    var npc = Terraria.Main.npc[testId];
        //                    if (npc.type != Terraria.ID.NPCID.Guide)
        //                    {
        //                        Logging.ProgramLog.Debug.Log("NPC type is now Terraria.ID.NPCID.Guide");
        //                        npc.type = Terraria.ID.NPCID.Guide;
        //                        Logging.ProgramLog.Log("Main.npcFrameCount: " + Main.npcFrameCount[npc.type]);
        //                    }
        //                }
        //                else if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.E))
        //                {
        //                    var npc = Terraria.Main.npc[testId] as OTANpc;
        //                    if (npc != null && npc._emulateNPCTypeId != Terraria.ID.NPCID.Guide)
        //                    {
        //                        Logging.ProgramLog.Debug.Log("Now emulating Guide");
        //                        npc._emulateNPCTypeId = Terraria.ID.NPCID.Guide;
        //                    }
        //                }
        //                else if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.R))
        //                {
        //                    var npc = Terraria.Main.npc[testId] as OTANpc;
        //                    if (npc != null && npc._emulateNPCTypeId != 0)
        //                    {
        //                        Logging.ProgramLog.Debug.Log("Not emulating Guide");
        //                        npc._emulateNPCTypeId = 0;
        //                    }
        //                }
        //            }
    }
}
#endif