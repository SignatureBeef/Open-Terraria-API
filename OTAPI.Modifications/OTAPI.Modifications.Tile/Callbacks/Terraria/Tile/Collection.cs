using OTAPI.Tile;
using System;
using System.Linq;
using System.Reflection.Emit;

namespace OTAPI.Core.Callbacks.Terraria
{
	internal static class Collection
	{
		public static ITileCollection Create()
		{
			//Fire the hook to allow a consumer to specify a custom implementation
			ITileCollection collection = Hooks.Tile.CreateCollection?.Invoke();

			//Either return the custom provider, or use the default one.
			return collection ?? new DefaultTileCollection();
		}

		private static Func<OTAPI.Tile.ITile> GetNewTile = GetNewTileMethod();

		/// <summary>
		/// Creates an ITile instance using the defaul Terraria.Tile constructor.
		/// 
		/// This cannot be performed at compile time using the OTAPI solution as
		/// Terraria.Tile does not implement ITile at compile time.
		/// </summary>
		/// <returns>ITile instance</returns>
		private static Func<OTAPI.Tile.ITile> GetNewTileMethod()
		{
			var dm = new DynamicMethod("GetTileCollection", typeof(OTAPI.Tile.ITile), null);
			var processor = dm.GetILGenerator();

			processor.Emit(OpCodes.Newobj, typeof(global::Terraria.Tile).GetConstructors().Single(x => x.GetParameters().Length == 0));
			processor.Emit(OpCodes.Ret);

			return (Func<OTAPI.Tile.ITile>)dm.CreateDelegate(typeof(Func<OTAPI.Tile.ITile>));
		}

		/// <summary>
		/// Replaces all Terraria.Tile initialisation calls in the terraria assembly.
		/// 
		/// If null is returned from the handler, a default Terraria.Tile instance will be created.
		/// </summary>
		/// <returns>An instance of ITile</returns>
		internal static ITile CreateTile()
		{
			return Hooks.Tile.CreateTile?.Invoke() ?? GetNewTile();
		}
	}
}
