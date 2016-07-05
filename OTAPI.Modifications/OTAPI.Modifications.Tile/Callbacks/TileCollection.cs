namespace OTAPI.Tile
{
	public interface ITileCollection
	{
		Terraria.Tile this[int x, int y] { get; set; }
	}

	public class TileCollection : ITileCollection
	{
		protected Terraria.Tile[,] _tiles;

		internal TileCollection() : this(new Terraria.Tile[Terraria.Main.maxTilesX, Terraria.Main.maxTilesY])
		{

		}

		protected TileCollection(Terraria.Tile[,] collection)
		{
			_tiles = collection;
		}

		public virtual Terraria.Tile this[int x, int y]
		{
			get
			{
				return _tiles[x, y];
			}
			set
			{
				_tiles[x, y] = value;
			}
		}
	}
}
