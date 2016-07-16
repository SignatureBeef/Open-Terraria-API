using OTAPI.Tile;

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
	}
}
