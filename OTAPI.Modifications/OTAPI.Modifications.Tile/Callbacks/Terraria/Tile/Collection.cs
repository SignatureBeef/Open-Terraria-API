using OTAPI.Tile;

namespace OTAPI.Core.Callbacks.Terraria
{
	internal static class Collection
	{
		public static ITileCollection Create()
		{
			//Call hook
			return new TileCollection();
		}
	}
}
