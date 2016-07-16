using OTAPI.Tile;

namespace OTAPI.Core
{
	public static partial class Hooks
	{
		public static partial class Tile
		{
			#region Handlers
			public delegate ITileCollection CreateCollectionHandler();
			#endregion

			/// <summary>
			/// Occurs on the static constructor of Terraria.Main when
			/// the Terraria.Main.tile field is being initialised.
			/// Use this to create custom tile providers.
			/// </summary>
			public static CreateCollectionHandler CreateCollection;
		}
	}
}
