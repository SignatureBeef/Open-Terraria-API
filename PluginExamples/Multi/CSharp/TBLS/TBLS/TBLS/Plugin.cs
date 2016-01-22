using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OTA.Mod;
using OTA.Plugin;
using System;
using Terraria;

namespace TBLS
{
    [OTAVersion(1, 0)]
    public class Plugin : BasePlugin
    {
        public Plugin()
        {
            this.Author = "DeathCradle";
            this.Description = "Client hook testing plugin.";
            this.Name = "TBLS";
            this.Version = "1.0";
        }

        protected override void Initialized(object state)
        {
            base.Initialized(state);
        }

        protected override void Enabled()
        {
            base.Enabled();
            OTA.Logging.ProgramLog.Log($"{base.Name} enabled");
        }

        [Hook]
        void OnPlayerEnter(ref HookContext ctx, ref HookArgs.PlayerEnteredGame args)
        {
            OTA.Logging.ProgramLog.Log($"Spawning {Sassy.SassyName}");
            NPC.NewNPC((int)(ctx.Player.position.X), (int)(ctx.Player.position.Y), EntityRegistrar.Npcs[Sassy.SassyName]);
        }

        bool state;

        [Hook]
        void OnUpdate(ref HookContext ctx, ref HookArgs.GameUpdate args)
        {
            Main.ignoreErrors = false;
            if (args.State == MethodState.Begin)
            {
                var mouse = Terraria.Main.mouseState;

                if (mouse.RightButton == ButtonState.Pressed)
                {
                    if (!state)
                    {
                        state = true;

                        if (Terraria.Main.myPlayer > -1 && Terraria.Main.myPlayer < 10 && Terraria.Main.player[Terraria.Main.myPlayer] != null)
                        {
                            var player = Terraria.Main.player[Terraria.Main.myPlayer];
                            var tileType = EntityRegistrar.Tiles[TestTile.TestTileName];

                            var playerX = (int)(player.position.X / 16f);
                            var playerY = (int)(player.position.Y / 16f) - 4;

                            Console.WriteLine($"Tiles near: {playerX},{playerY}");

                            Terraria.WorldGen.PlaceTile(playerX, playerY, tileType);
                            Terraria.WorldGen.PlaceTile(playerX + 1, playerY, Terraria.ID.TileID.Stone);

                            Terraria.Main.dayTime = true;
                            Terraria.Main.time = OTA.Command.WorldTime.Parse("8:00am").Value.GameTime;
                        }
                    }
                }
                else state = false;
            }
        }

		#if !SERVER
        [Hook]
        void OnDraw(ref HookContext ctx, ref HookArgs.GameDraw args)
        {
            if (args.State == MethodState.End && Terraria.Main.myPlayer > -1 && Terraria.Main.myPlayer < 10 && Terraria.Main.player[Terraria.Main.myPlayer] != null)
            {
                if (Terraria.Main.tile != null && Main.selectedWorld > -1)
                {
                    int tileX = (int)((Main.mouseX + Main.screenPosition.X) / 16f);
                    int tileY = (int)((Main.mouseY + Main.screenPosition.Y) / 16f);

                    var tile = Terraria.Main.tile[tileX, tileY];
                    if (tile != null)
                    {
                        if (tile.type > 0)
                        {
                            var x = Main.mouseState.X;
                            var y = Main.mouseState.Y;

                            var text = "Tile type: " + tile.type;
                            var font = Terraria.Main.fontMouseText;
                            var pos = new Vector2(x, y);
                            var origin = Vector2.Zero;
                            var color = Color.White;
                            
                            Terraria.Main.spriteBatch.DrawString(font, text, pos, color, 0, origin, 1, SpriteEffects.None, 0);
                        }
                    }
                }
            }
        }
		#endif
    }
}

