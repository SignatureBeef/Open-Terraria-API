using System;
using OTA.Client.Npc;
using OTA.Client;
using Terraria;
using OTA.Plugin;
using Microsoft.Xna.Framework.Input;
using OTA.Client.Tile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BLS
{
	[NativeMod (TestTileName)]
	public class TestTile : OTATile
	{
		public const String TestTileName = "Test tile";

		public override void OnSetDefaults ()
		{
			Console.WriteLine ($"Initialising {TestTileName}");
			EmulateTile (Terraria.ID.TileID.Cloud);
		}
	}

	[OTAVersion (1, 0)]
	public class BLSPlugin : BasePlugin
	{
		public BLSPlugin ()
		{
			base.Author = "DeathCradle";
			base.Description = "BLS";
			base.Name = "BLS";
			base.Version = "0.1";
		}

		protected override void Enabled ()
		{
			base.Enabled ();
			OTA.Logging.ProgramLog.Log ($"{base.Name} enabled");
		}

		[Hook]
		void OnPlayerEnter (ref HookContext ctx, ref HookArgs.PlayerEnteredGame args)
		{
			OTA.Logging.ProgramLog.Log ($"Spawning {Sassy.SassyName}");
			NPC.NewNPC ((int)(ctx.Player.position.X), (int)(ctx.Player.position.Y), EntityRegistrar.Npcs [Sassy.SassyName]);
		}

		bool state;

		[Hook]
		void OnUpdate (ref HookContext ctx, ref HookArgs.GameUpdate args)
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

						if (Terraria.Main.myPlayer > -1 && Terraria.Main.myPlayer < 10 && Terraria.Main.player [Terraria.Main.myPlayer] != null)
						{
							var player = Terraria.Main.player [Terraria.Main.myPlayer];
							var tileType = EntityRegistrar.Tiles [TestTile.TestTileName];

							var playerX = (int)(player.position.X / 16f);
							var playerY = (int)(player.position.Y / 16f) - 4;

							Console.WriteLine ($"Tiles near: {playerX},{playerY}");

							Terraria.WorldGen.PlaceTile (playerX, playerY, tileType);
							Terraria.WorldGen.PlaceTile (playerX + 1, playerY, Terraria.ID.TileID.Stone);

							Terraria.Main.dayTime = true;
							Terraria.Main.time = OTA.Command.WorldTime.Parse ("8:00am").Value.GameTime;
						}
					}
				}
				else state = false;
			}
		}

		[Hook]
		void OnDraw (ref HookContext ctx, ref HookArgs.GameDraw args)
		{
			if (args.State == MethodState.End && Terraria.Main.myPlayer > -1 && Terraria.Main.myPlayer < 10 && Terraria.Main.player [Terraria.Main.myPlayer] != null)
			{
				if (Terraria.Main.tile != null && Main.selectedWorld > -1)
				{
					int tileX = (int)((Main.mouseX + Main.screenPosition.X) / 16f);
					int tileY = (int)((Main.mouseY + Main.screenPosition.Y) / 16f);

					var tile = Terraria.Main.tile [tileX, tileY];
					if (tile != null)
					{
						if (tile.type > 0)
						{
							var x = Main.mouseState.X;
							var y = Main.mouseState.Y;

							var text = "Tile type: " + tile.type;
							var font = Terraria.Main.fontMouseText;
							var pos = new Vector2 (x, y);
							var origin = Vector2.Zero;
							var color = Color.White;

							Terraria.Main.spriteBatch.DrawString (font, text, pos, color, 0, origin, 1, SpriteEffects.None, 0);
						}
					}
				}
			}
		}
	}

	[NativeMod (SassyName)]
	public class Sassy : OTANpc
	{
		public const string SassyName = "Sassy";

		int shopId;

		public override void OnSetDefaults ()
		{
			base.OnSetDefaults ();

			EmulateNPC (Terraria.ID.NPCID.Merchant, true);

			LoadTexture ("sassy", true);

			Npc.IsTownNpc = true;
			Npc.townNPC = true;
			Npc.friendly = true;
			Npc.width = 18;
			Npc.height = 40;
			Npc.aiStyle = 7;
			Npc.damage = 10;
			Npc.defense = 15;
			Npc.lifeMax = 250;
			Npc.soundHit = 1;
			Npc.soundKilled = 1;
			Npc.knockBackResist = 0.5f;

			SetName (SassyName);

			KillCount = 0;
			Catchable = false;
			FrameCount = 16;

			shopId = EntityRegistrar.Shops.Register (new SassyFoodsShop ());
		}

		public override string OnChat ()
		{
			return ".....wadiyatalkinabeet";
		}

		public override string[] OnGetChatButtons ()
		{
			/* Close is always in the middle  */
			return new string[] { "Sassy Foods" };
		}

		public override bool OnChatButtonClick (OTA.Callbacks.NpcChatButton button)
		{
			if (button == OTA.Callbacks.NpcChatButton.First)
			{
				OpenSassyFoods ();
			}
			return base.OnChatButtonClick (button);
		}

		#region "Sassy foods"

		private void OpenSassyFoods ()
		{
			Main.playerInventory = true;
			Main.npcChatText = string.Empty;
			Main.npcShop = shopId;
			Main.instance.shop [Main.npcShop].SetupShop (shopId);
			Main.PlaySound (12, -1, -1, 1);
		}

		#endregion
	}
}

