using System;
using System.Linq;
using System.Reflection.Emit;

namespace OTAPI.Tile
{
	/// <summary>
	/// Replicates the default functionality of a normal array.
	/// </summary>
	public class DefaultTileCollection : ITileCollection
	{
		protected ITile[,] _tiles;

		public int Width => this._tiles.GetLength(0);

		public int Height => this._tiles.GetLength(1);

		/// <summary>
		/// Creates a new 2D array instance of ITile using the default Terraria.Tile implementation.
		/// 
		/// This cannot be compiled in the OTAPI solution as the Terraria.Tile will not implement 
		/// ITile at compile time.
		/// </summary>
		/// <returns>A 2D ITile array instance</returns>
		private static ITile[,] GetNewTileCollection()
		{
			var dm = new DynamicMethod("GetNewTileCollection", typeof(ITile[,]), null);
			var processor = dm.GetILGenerator();

			processor.Emit(OpCodes.Ldsfld, typeof(global::Terraria.Main).GetField("maxTilesX"));
			processor.Emit(OpCodes.Ldsfld, typeof(global::Terraria.Main).GetField("maxTilesY"));
			processor.Emit(OpCodes.Newobj, typeof(global::Terraria.Tile[,]).GetConstructors().Single(x => x.GetParameters().Length == 2));
			processor.Emit(OpCodes.Ret);

			return (ITile[,])dm.Invoke(null, null);
		}

		/// <summary>
		/// Replicates the default terraria tile array constructor
		/// </summary>
		internal DefaultTileCollection() : this(GetNewTileCollection()) { }

		/// <summary>
		/// Initializes a new instance with the specified collection
		/// as the underlying source.
		/// </summary>
		/// <param name="collection"></param>
		protected DefaultTileCollection(ITile[,] collection)
		{
			_tiles = collection;
		}

		/// <summary>
		/// Describes the get and set tile accessors that the vanilla server will use
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public virtual ITile this[int x, int y]
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
