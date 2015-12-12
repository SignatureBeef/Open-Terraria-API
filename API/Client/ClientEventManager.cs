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

            OTANpc.ResizeArrays();
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
        void OnNewItem(ref HookContext ctx, ref Plugin.HookArgs.NewItem args)
        {
            var mod = EntityRegistrar.Items.Create(args.Type);
            if (mod != null)
            {
                var item = new Terraria.Item();
                item.Mod = mod;
                mod.Item = item;
                mod.Initialise();
                ctx.SetResult(HookResult.RECTIFY, true, item);
            }
        }

        [Hook]
        void OnItemSetDefaultsByName(ref HookContext ctx, ref Plugin.HookArgs.ItemSetDefaultsByName args)
        {
            if (args.State == MethodState.Begin)
            {
                var mod = EntityRegistrar.Items.Create(args.Name);
                if (mod != null)
                {
                    args.Item.Mod = mod;
                    mod.Item = args.Item;
                    mod.Initialise();
                    ctx.SetResult(HookResult.RECTIFY);
                }
            }
        }

        [Hook]
        void OnNewNpc(ref HookContext ctx, ref Plugin.HookArgs.NewNpc args)
        {
            var mod = EntityRegistrar.Npcs.Create(args.Type);
            if (mod != null)
            {
                var npc = new Terraria.NPC();
                npc.Mod = mod;
                mod.Npc = npc;
                mod.Initialise();
                ctx.SetResult(HookResult.RECTIFY, true, npc);
            }
        }

        [Hook]
        void OnNpcDraw(ref HookContext ctx, ref Plugin.HookArgs.NpcDraw args)
        {
            Terraria.Main.ignoreErrors = false;
            var ota = args.Npc.Mod as OTANpc;
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
            var ota = args.Npc.Mod as OTANpc;
            if (ota != null)
            {
                args.Npc.whoAmI = args.NpcIndex;

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
            var ota = args.Npc.Mod as OTANpc;
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
            var ota = args.Npc.Mod as OTANpc;
            if (ota != null)
            {
                ctx.SetResult(HookResult.IGNORE, true, ota.OnChat());
            }
        }

        [Hook]
        void OnNpcChatButtons(ref HookContext ctx, ref Plugin.HookArgs.NpcGetChatButtons args)
        {
            var ota = args.Npc.Mod as OTANpc;
            if (ota != null)
            {
                var buttons = ota.OnGetChatButtons();
                if (buttons != null)
                {
                    ctx.SetResult(HookResult.RECTIFY, true, buttons);
                }
            }
        }

        [Hook]
        void OnNpcChatButtonClicked(ref HookContext ctx, ref Plugin.HookArgs.NpcChatButtonClick args)
        {
            var ota = args.Npc.Mod as OTANpc;
            if (ota != null)
            {
                var proceed = ota.OnChatButtonClick(args.Button);
                if (!proceed)
                {
                    ctx.SetResult(HookResult.IGNORE);
                }
            }
        }

        [Hook]
        void OnChestSetupShop(ref HookContext ctx, ref Plugin.HookArgs.ChestSetupShop args)
        {
            if (args.State == MethodState.End)
            {
                var shop = EntityRegistrar.Shops.Find(args.Type);
                if (shop != null)
                {
                    args.Chest.Mod = shop;
                    shop.OnInitialise(args.Chest);
                }
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