#if CLIENT
using System;

using Terraria;

using OTA.Plugin;
using OTA.Client.Npc;
using OTA.Plugin;

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
        void OnPluginLoad(ref HookContext ctx, ref HookArgs.PluginEnabled args)
        {
            if (null != args.Plugin.Assembly)
                EntityRegistrar.ScanAssembly(args.Plugin.Assembly);
        }

        [Hook]
        void OnInitialised(ref HookContext ctx, ref HookArgs.GameInitialize args)
        {
            if (args.State == MethodState.End)
            {
                Client.Tile.OTATile.ResizeArrays(true, false);
                EntityRegistrar.Tiles.InitialiseTiles();
            }
        }

        [Hook]
        void OnGUIChatBoxOpen(ref HookContext ctx, ref HookArgs.GUIChatBoxOpen args)
        {
            ctx.SetResult(HookResult.RECTIFY, true, 
                args.IsEnterDown
                && !args.IsLeftAltDown
                && !args.IsRightAltDown
                && Terraria.Main.hasFocus);
        }

        #region Item

        [Hook]
        void OnNewItem(ref HookContext ctx, ref HookArgs.NewItem args)
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
        void OnItemSetDefaultsByName(ref HookContext ctx, ref HookArgs.ItemSetDefaultsByName args)
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
        void OnItemSetDefaultsByType(ref HookContext ctx, ref HookArgs.ItemSetDefaultsByType args)
        {
            if (args.State == MethodState.Begin)
            {
                var mod = EntityRegistrar.Items.Create(args.Type);
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
        void OnItemNetDefaults(ref HookContext ctx, ref HookArgs.ItemNetDefaults args)
        {
            if (args.State == MethodState.Begin)
            {
                var mod = EntityRegistrar.Items.Create(args.Type);
                if (mod != null)
                {
                    args.Item.Mod = mod;
                    mod.Item = args.Item;
                    mod.Initialise();
                    ctx.SetResult(HookResult.RECTIFY);
                }
            }
        }

        #endregion

        #region Npc

        [Hook]
        void OnNewNpc(ref HookContext ctx, ref HookArgs.NewNpc args)
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
        void OnNpcDraw(ref HookContext ctx, ref HookArgs.NpcDraw args)
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
        void OnNpcUpdate(ref HookContext ctx, ref HookArgs.NpcUpdate args)
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
        void OnNpcAI(ref HookContext ctx, ref HookArgs.NpcAI args)
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
        void OnNpcChat(ref HookContext ctx, ref HookArgs.NpcGetChat args)
        {
            var ota = args.Npc.Mod as OTANpc;
            if (ota != null)
            {
                ctx.SetResult(HookResult.IGNORE, true, ota.OnChat());
            }
        }

        [Hook]
        void OnNpcChatButtons(ref HookContext ctx, ref HookArgs.NpcGetChatButtons args)
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
        void OnNpcChatButtonClicked(ref HookContext ctx, ref HookArgs.NpcChatButtonClick args)
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
        void OnNpcPreSpawn(ref HookContext ctx, ref HookArgs.NpcPreSpawn args)
        {
            //Test whether to spawn a OTANpc or let vanilla spawn it's own.
        }

        #endregion

        #region Shop

        [Hook]
        void OnChestSetupShop(ref HookContext ctx, ref HookArgs.ChestSetupShop args)
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

        #endregion

        #region Projectile

        [Hook]
        void OnNewProjectile(ref HookContext ctx, ref HookArgs.NewProjectile args)
        {
            var mod = EntityRegistrar.Projectiles.Create(args.Type);
            if (mod != null)
            {
                var proj = new Terraria.Projectile();
                proj.Mod = mod;
                mod.Projectile = proj;
                mod.Initialise();

                Terraria.Main.projectile[args.Index] = proj;

                ctx.SetResult(HookResult.RECTIFY, true, proj);
            }
        }

        #endregion
    }
}
#endif