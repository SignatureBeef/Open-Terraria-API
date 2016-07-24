using Microsoft.Xna.Framework;

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
		ITile this[int x, int y] { get; set; }
	}

	/// <summary>
	/// ITile is injected into the Terraria assembly to replace all instance usages
	/// of Terraria.Tile.
	/// 
	/// This allows us to seamlessly swap out implementations of the tile mechanism.
	/// </summary>
	/// <remarks>This interface is based of the implementations after the Terraria.Tile
	/// fields have been changed to properties.</remarks>
	public interface ITile
	{
		int collisionType { get; }
		ushort type { get; set; }
		byte wall { get; set; }
		byte liquid { get; set; }
		short sTileHeader { get; set; }
		byte bTileHeader { get; set; }
		byte bTileHeader2 { get; set; }
		byte bTileHeader3 { get; set; }
		short frameX { get; set; }
		short frameY { get; set; }

		object Clone();
		void ClearEverything();
		void ClearTile();
		void CopyFrom(ITile from);
		bool isTheSameAs(ITile compTile);
		int blockType();
		void liquidType(int liquidType);
		byte liquidType();
		bool nactive();
		void ResetToType(ushort type);
		void ClearMetadata();
		Color actColor(Color oldColor);
		bool topSlope();
		bool bottomSlope();
		bool leftSlope();
		bool rightSlope();
		bool HasSameSlope(ITile tile);
		byte wallColor();
		void wallColor(byte wallColor);
		bool lava();
		void lava(bool lava);
		bool honey();
		void honey(bool honey);
		bool wire4();
		void wire4(bool wire4);
		int wallFrameX();
		void wallFrameX(int wallFrameX);
		byte frameNumber();
		void frameNumber(byte frameNumber);
		byte wallFrameNumber();
		void wallFrameNumber(byte wallFrameNumber);
		int wallFrameY();
		void wallFrameY(int wallFrameY);
		bool checkingLiquid();
		void checkingLiquid(bool checkingLiquid);
		bool skipLiquid();
		void skipLiquid(bool skipLiquid);
		byte color();
		void color(byte color);
		bool active();
		void active(bool active);
		bool inActive();
		void inActive(bool inActive);
		bool wire();
		void wire(bool wire);
		bool wire2();
		void wire2(bool wire2);
		bool wire3();
		void wire3(bool wire3);
		bool halfBrick();
		void halfBrick(bool halfBrick);
		bool actuator();
		void actuator(bool actuator);
		byte slope();
		void slope(byte slope);

		string ToString();
	}
}
