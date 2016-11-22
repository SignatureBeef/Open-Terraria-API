using OTAPI.Tile;

namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class Tile
		{
			#region Handlers
			public delegate ITileCollection CreateCollectionHandler();
			public delegate ITile CreateTileHandler();
			public delegate ITile CreateTileFromHandler(ITile copy);
			#endregion

			/// <summary>
			/// Occurs on the static constructor of Terraria.Main when
			/// the Terraria.Main.tile field is being initialised.
			/// Use this to create custom tile providers.
			/// </summary>
			public static CreateCollectionHandler CreateCollection;

			/// <summary>
			/// Occurs whenever a tile instance is to be created
			/// in the Terraria assembly.
			/// </summary>
			public static CreateTileHandler CreateTile;

			/// <summary>
			/// Occurs whenever a tile instance is to be created
			/// in the Terraria assembly.
			/// </summary>
			public static CreateTileFromHandler CreateTileFrom;
		}
	}
}
