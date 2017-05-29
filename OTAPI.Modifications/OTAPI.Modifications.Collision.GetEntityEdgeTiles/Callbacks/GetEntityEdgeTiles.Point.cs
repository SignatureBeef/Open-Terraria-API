using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Terraria;
using OTAPI.Tile;

namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Collision
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

		internal static Point GetEntityEdgeTilePoint(int x, int y)
		{
			var collection = GetTileCollection();

			x = Math.Max(0, x);
			x = Math.Min(collection.Width, x);

			y = Math.Max(0, y);
			y = Math.Min(collection.Width, y);

			return new Point(x, y);
		}
	}
}
