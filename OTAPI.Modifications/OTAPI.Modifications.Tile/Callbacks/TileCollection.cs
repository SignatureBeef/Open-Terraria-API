namespace OTAPI.Tile
{
	/// <summary>
	/// Replicates the default functionality of a normal array.
	/// </summary>
	public class TileCollection : ITileCollection
	{
		protected Terraria.Tile[,] _tiles;

		/// <summary>
		/// Replicates the default terraria tile array constructor
		/// </summary>
		internal TileCollection() : this(new Terraria.Tile[Terraria.Main.maxTilesX, Terraria.Main.maxTilesY])
		{

		}

		/// <summary>
		/// Initializes a new instance with the specified collection
		/// as the underlying source.
		/// </summary>
		/// <param name="collection"></param>
		protected TileCollection(Terraria.Tile[,] collection)
		{
			_tiles = collection;
		}

		/// <summary>
		/// Describes the get and set tile accessors that the vanilla server will use
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
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
