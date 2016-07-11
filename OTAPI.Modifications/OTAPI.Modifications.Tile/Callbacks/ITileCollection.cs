namespace OTAPI.Tile
{
	/// <summary>
	/// Describes accessors to a terraria tile collection.
	/// This class is injected into the terraria assembly to replace
	/// all occurrences of the Terraria.Main.tile array.
	/// </summary>
	public interface ITileCollection
	{
		/// <summary>
		/// Describes the get and set tile accessors that the vanilla server will use
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		Terraria.Tile this[int x, int y] { get; set; }
	}
}
