using System;
using OTA.Client.Npc;
using OTA.Client;
using Terraria;
using OTA.Plugin;
using Microsoft.Xna.Framework.Input;
using OTA.Client.Tile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TBLS
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

		public override bool OnChatButtonClick (NpcChatButton button)
		{
			if (button == NpcChatButton.First)
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

		public override double OnPreSpawn (HookArgs.NpcPreSpawn info)
		{
			//Spawn during day
			if (!Main.dayTime) return 0;

//			const Int32 SpawnRange = 5;
//			if (info.SpawnTileX > Main.spawnTileX + SpawnRange
//				|| info.SpawnTileX < Main.spawnTileX - SpawnRange
//				|| info.SpawnTileY < Main.spawnTileY - SpawnRange
//				|| info.SpawnTileY > Main.spawnTileY + SpawnRange) return 0;

			if (WorldGen.numIslandHouses > 0)
			{

			}

			return 0.25;
		}

		#endregion
	}
}

