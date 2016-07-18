using System;
using System.Reflection.Emit;

namespace OTAPI.Core.Callbacks.Terraria
{
	internal static partial class WorldGen
	{
		private static Func<OTAPI.Tile.ITileCollection> GetTileCollection = GetTileCollectionMethod();

		public static Func<OTAPI.Tile.ITileCollection> GetTileCollectionMethod()
		{
			var dm = new DynamicMethod("GetTileCollection", typeof(OTAPI.Tile.ITileCollection), null);
			var processor = dm.GetILGenerator();

			processor.Emit(OpCodes.Ldsfld, Type.GetType("Terraria.Main").GetField("tile"));
			processor.Emit(OpCodes.Ret);

			return (Func<OTAPI.Tile.ITileCollection>)dm.CreateDelegate(typeof(Func<OTAPI.Tile.ITileCollection>));
		}

		/// <summary>
		/// Replaces all tile type updates in Terraria.WorldGen.hardmodeTileUpdate
		/// </summary>
		internal static bool HardmodeTileUpdate(int x, int y, ushort type)
		{
			if (Hooks.World.HardmodeTileUpdate?.Invoke(x, y, ref type) == HookResult.Cancel)
				return false;

			//HardmodeTileUpdate replaces the below code in the vanilla method.
			//We must reapply the logic (besides, we allow type altering, so
			//we need this anyway)
			GetTileCollection()[x, y].type = type;
			return true;
		}

		internal static bool HardmodeTilePlace(int i, int j, int type, bool mute, bool forced, int plr, int style)
		{
			var usType = (ushort)type;
			if (Hooks.World.HardmodeTileUpdate?.Invoke(i, j, ref usType) == HookResult.Cancel)
				return false;

			global::Terraria.WorldGen.PlaceTile(i, j, usType, mute, forced, plr, style);
			return true;
		}
	}
}
