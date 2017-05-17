using System;
using System.Reflection.Emit;
using Vanilla = global::Terraria;

namespace OTAPI.Callbacks.Terraria
{
	internal static partial class WorldGen
	{
		private static Func<OTAPI.Tile.ITileCollection> GetTileCollection = GetTileCollectionMethod();

		/// <summary>
		/// Gets active ITileCollection instance as it's not available at compile time.
		/// 
		/// This is because at compile time we are using Terraria.Main.tile which is a
		/// 2D array of Terraria.Tile which uses fields. 
		/// If we compile against fields in OTAPI then our IL wont match after the tile
		/// modifications are applied.
		/// </summary>
		/// <returns>ITileCollection instance</returns>
		private static Func<OTAPI.Tile.ITileCollection> GetTileCollectionMethod()
		{
			var dm = new DynamicMethod("GetTileCollectionInstance", typeof(OTAPI.Tile.ITileCollection), null);
			var processor = dm.GetILGenerator();

			processor.Emit(OpCodes.Ldsfld, typeof(global::Terraria.Main).GetField("tile"));
			processor.Emit(OpCodes.Ret);

			return (Func<OTAPI.Tile.ITileCollection>)dm.CreateDelegate(typeof(Func<OTAPI.Tile.ITileCollection>));
		}

		/// <summary>
		/// Replaces all tile type updates in Terraria.WorldGen.hardmodeTileUpdate
		/// </summary>
		internal static bool HardmodeTileUpdate(int x, int y, ushort type)
		{
			var result = Hooks.World.HardmodeTileUpdate?.Invoke(x, y, ref type);

			if (result == HardmodeTileUpdateResult.Cancel)
				return false;

			else if (result == null || result == HardmodeTileUpdateResult.Continue)
			{
				//HardmodeTileUpdate replaces the below code in the vanilla method.
				//We must reapply the logic (besides, we allow type altering, so
				//we need this anyway)
				GetTileCollection()[x, y].type = type;

				Vanilla.WorldGen.SquareTileFrame(x, y, true);

				if (Vanilla.Main.netMode == 2)
					Vanilla.NetMessage.SendTileSquare(-1, x, y, 1);
			}
			return true;
		}

		internal static bool HardmodeTilePlace(int i, int j, int type, bool mute, bool forced, int plr, int style)
		{
			var usType = (ushort)type;
			var result = Hooks.World.HardmodeTileUpdate?.Invoke(i, j, ref usType);

			if (result == HardmodeTileUpdateResult.Cancel)
				return false;

			else if (result == HardmodeTileUpdateResult.Continue)
			{
				Vanilla.WorldGen.PlaceTile(i, j, usType, mute, forced, plr, style);
				if (Vanilla.Main.netMode == 2)
					Vanilla.NetMessage.SendTileSquare(-1, i, j, 1);
			}

			return true;
		}
	}
}
