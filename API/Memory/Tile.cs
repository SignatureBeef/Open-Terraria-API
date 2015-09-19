#define TESTING

using Microsoft.Xna.Framework;

namespace OTA.Memory
{
    /// <summary>
    /// Terrarian tile data collection
    /// </summary>
    public class TileCollection
    {
        internal TileData[,] data;

        public TileCollection(int X, int Y)
        {
            Logging.ProgramLog.Log("Creating tile array of {0}x{1}, {2}MB", X, Y, System.Runtime.InteropServices.Marshal.SizeOf(typeof(TileData)) * X * Y / 1024 / 1024);
            data = new TileData[X, Y];
        }

        public int SizeX
        {
            get { return data.GetLength(0); }
        }

        public int SizeY
        {
            get { return data.GetLength(1); }
        }

        public TileRef At(int x, int y)
        {
            return new TileRef(x, y);
        }

        public TileRef CreateTileAt(int x, int y)
        {
            data[x, y] = default(TileData);
            return new TileRef(x, y);
        }

        public void RemoveTileAt(int x, int y)
        {
            data[x, y] = default(TileData);
        }

        public MemTile this[int x, int y]
        {
            get
            { return new TileRef(x, y); }
            set
            { SetTile(x, y, value); }
        }

        //public static void SetWorldTile(int x, int y, MemTile tile)
        //{
        //    Terraria.Main.tile.SetTile(tile, x, y);
        //}

        //public static MemTile GetWorldTile(int x, int y)
        //{
        //    return Terraria.Main.tile.GetTile(x, y);
        //}

        public void SetTile(int x, int y, MemTile tile)
        {
            data[x, y]._wall = tile._wall;
            data[x, y]._type = tile._type;
            data[x, y]._sTileHeader = tile._sTileHeader;
            data[x, y]._liquid = tile._liquid;
            data[x, y]._frameY = tile._frameY;
            data[x, y]._frameX = tile._frameX;
            data[x, y]._bTileHeader3 = tile._bTileHeader3;
            data[x, y]._bTileHeader2 = tile._bTileHeader2;
            data[x, y]._bTileHeader = tile._bTileHeader;
        }

        public MemTile GetTile(int x, int y)
        {
            return new TileRef(x, y);
        }

        public static implicit operator TileCollection(MemTile[,] set)
        {
            return new TileCollection(set.GetLength(0), set.GetLength(1));
        }
    }

    /// <summary>
    /// The actual tile to be used by Terraria and plugins
    /// </summary>
    /// <remarks>This replaces Terraria.Tile and supersedes OTA.Memory.MemTile</remarks>
    public sealed class TileRef : MemTile
    {
        readonly int x, y;

        public TileRef(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        #if Full_API
        public override byte _wall
        {
            get
            { return Terraria.Main.tile.data[x, y]._wall; }
            set { Terraria.Main.tile.data[x, y]._wall = value; }
        }

        public override byte _liquid
        {
            get
            { return Terraria.Main.tile.data[x, y]._liquid; }
            set { Terraria.Main.tile.data[x, y]._liquid = value; }
        }

        public override byte _bTileHeader
        {
            get
            { return Terraria.Main.tile.data[x, y]._bTileHeader; }
            set { Terraria.Main.tile.data[x, y]._bTileHeader = value; }
        }

        public override byte _bTileHeader2
        {
            get
            { return Terraria.Main.tile.data[x, y]._bTileHeader2; }
            set { Terraria.Main.tile.data[x, y]._bTileHeader2 = value; }
        }

        public override byte _bTileHeader3
        {
            get
            { return Terraria.Main.tile.data[x, y]._bTileHeader3; }
            set { Terraria.Main.tile.data[x, y]._bTileHeader3 = value; }
        }

        public override short _frameX
        {
            get
            { return Terraria.Main.tile.data[x, y]._frameX; }
            set { Terraria.Main.tile.data[x, y]._frameX = value; }
        }

        public override short _frameY
        {
            get
            { return Terraria.Main.tile.data[x, y]._frameY; }
            set { Terraria.Main.tile.data[x, y]._frameY = value; }
        }

        public override short _sTileHeader
        {
            get
            { return Terraria.Main.tile.data[x, y]._sTileHeader; }
            set { Terraria.Main.tile.data[x, y]._sTileHeader = value; }
        }

        public override ushort _type
        {
            get
            { return Terraria.Main.tile.data[x, y]._type; }
            set { Terraria.Main.tile.data[x, y]._type = value; }
        }
        #endif
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Pack = 1)]
    public struct TileData
    {
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal byte _wall;

        [System.Runtime.InteropServices.FieldOffset(1)]
        internal byte _liquid;

        [System.Runtime.InteropServices.FieldOffset(2)]
        internal byte _bTileHeader;

        [System.Runtime.InteropServices.FieldOffset(3)]
        internal byte _bTileHeader2;

        [System.Runtime.InteropServices.FieldOffset(4)]
        internal byte _bTileHeader3;

        [System.Runtime.InteropServices.FieldOffset(5)]
        internal short _frameX;

        [System.Runtime.InteropServices.FieldOffset(7)]
        internal short _frameY;

        [System.Runtime.InteropServices.FieldOffset(9)]
        internal short _sTileHeader;

        [System.Runtime.InteropServices.FieldOffset(11)]
        internal ushort _type;

        public void SetWall(byte value)
        {
            this._wall = value;
        }
        public void SetLiquid(byte value)
        {
            this._liquid = value;
        }
        public void SetBTileHeader(byte value)
        {
            this._bTileHeader = value;
        }
        public void SetBTileHeader2(byte value)
        {
            this._bTileHeader2 = value;
        }
        public void SetBTileHeader3(byte value)
        {
            this._bTileHeader3 = value;
        }
        public void SetFrameX(short value)
        {
            this._frameX = value;
        }
        public void SetFrameY(short value)
        {
            this._frameY = value;
        }
        public void SetSTileHeader(short value)
        {
            this._sTileHeader = value;
        }
        public void SetType(ushort value)
        {
            this._type = value;
        }
    }

    /// <summary>
    /// Overridable terrarian tile. 
    /// [TODO] Allow use from SQL and so on, contact DeathCradle if you require this.
    /// </summary>
    public class MemTile
    {
        public const System.Int32 TileDataSize = 13;

        public virtual byte _wall { get; set; }
        public virtual byte _liquid { get; set; }
        public virtual byte _bTileHeader { get; set; }
        public virtual byte _bTileHeader2 { get; set; }
        public virtual byte _bTileHeader3 { get; set; }
        public virtual short _sTileHeader { get; set; }
        public virtual short _frameX { get; set; }
        public virtual short _frameY { get; set; }
        public virtual ushort _type { get; set; }

        //private TileRef _ref;

        public byte wall
        {
            get
            { return _wall; }
            set { _wall = value; }
        }
        public byte liquid
        {
            get
            { return _liquid; }
            set { _liquid = value; }
        }
        public byte bTileHeader
        {
            get
            { return _bTileHeader; }
            set { _bTileHeader = value; }
        }
        public byte bTileHeader2
        {
            get
            { return _bTileHeader2; }
            set { _bTileHeader2 = value; }
        }
        public byte bTileHeader3
        {
            get
            { return _bTileHeader3; }
            set { _bTileHeader3 = value; }
        }

        public short sTileHeader
        {
            get
            { return _sTileHeader; }
            set { _sTileHeader = value; }
        }
        public ushort type
        {
            get
            { return _type; }
            set { _type = value; }
        }
        public short frameX
        {
            get
            { return _frameX; }
            set { _frameX = value; }
        }
        public short frameY
        {
            get
            { return _frameY; }
            set { _frameY = value; }
        }

        public int collisionType
        {
            get
            {
                if (!this.active())
                {
                    return 0;
                }
                if (this.halfBrick())
                {
                    return 2;
                }
                if (this.slope() > 0)
                {
                    return (int)(2 + this.slope());
                }
#if Full_API
                if (Terraria.Main.tileSolid[(int)this.type] && !Terraria.Main.tileSolidTop[(int)this.type])
                {
                    return 1;
                }
#endif
                return -1;
            }
        }

        public MemTile()
        {
            this.type = 0;
            this.wall = 0;
            this.liquid = 0;
            this.sTileHeader = 0;
            this.bTileHeader = 0;
            this.bTileHeader2 = 0;
            this.bTileHeader3 = 0;
            this.frameX = 0;
            this.frameY = 0;
        }

        public MemTile(MemTile copy)
        {
            if (copy == null)
            {
                this.type = 0;
                this.wall = 0;
                this.liquid = 0;
                this.sTileHeader = 0;
                this.bTileHeader = 0;
                this.bTileHeader2 = 0;
                this.bTileHeader3 = 0;
                this.frameX = 0;
                this.frameY = 0;
                return;
            }
            this.type = copy.type;
            this.wall = copy.wall;
            this.liquid = copy.liquid;
            this.sTileHeader = copy.sTileHeader;
            this.bTileHeader = copy.bTileHeader;
            this.bTileHeader2 = copy.bTileHeader2;
            this.bTileHeader3 = copy.bTileHeader3;
            this.frameX = copy.frameX;
            this.frameY = copy.frameY;
        }

        public object Clone()
        {
            return base.MemberwiseClone();
        }

        public void ClearEverything()
        {
            this.type = 0;
            this.wall = 0;
            this.liquid = 0;
            this.sTileHeader = 0;
            this.bTileHeader = 0;
            this.bTileHeader2 = 0;
            this.bTileHeader3 = 0;
            this.frameX = 0;
            this.frameY = 0;
        }

        public void ClearTile()
        {
            this.slope(0);
            this.halfBrick(false);
            this.active(false);
        }

        public void CopyFrom(MemTile from)
        {
            this.type = from.type;
            this.wall = from.wall;
            this.liquid = from.liquid;
            this.sTileHeader = from.sTileHeader;
            this.bTileHeader = from.bTileHeader;
            this.bTileHeader2 = from.bTileHeader2;
            this.bTileHeader3 = from.bTileHeader3;
            this.frameX = from.frameX;
            this.frameY = from.frameY;
        }

        public bool isTheSameAs(MemTile compTile)
        {
            if (compTile == null)
            {
                return false;
            }
            if (this.sTileHeader != compTile.sTileHeader)
            {
                return false;
            }
            if (this.active())
            {
                if (this.type != compTile.type)
                {
                    return false;
                }

#if Full_API
                if (Terraria.Main.tileFrameImportant[(int)this.type] && (this.frameX != compTile.frameX || this.frameY != compTile.frameY))
                {
                    return false;
                }
#endif
            }
            if (this.wall != compTile.wall || this.liquid != compTile.liquid)
            {
                return false;
            }
            if (compTile.liquid == 0)
            {
                if (this.wallColor() != compTile.wallColor())
                {
                    return false;
                }
            }
            else if (this.bTileHeader != compTile.bTileHeader)
            {
                return false;
            }
            return true;
        }

        public int wallFrameX()
        {
            return (int)((this.bTileHeader2 & 15) * 36);
        }

        public void wallFrameX(int wallFrameX)
        {
            this.bTileHeader2 = (byte)((int)(this.bTileHeader2 & 240) | (wallFrameX / 36 & 15));
        }

        public int wallFrameY()
        {
            return (int)((this.bTileHeader3 & 7) * 36);
        }

        public void wallFrameY(int wallFrameY)
        {
            this.bTileHeader3 = (byte)((int)(this.bTileHeader3 & 248) | (wallFrameY / 36 & 7));
        }

        public byte frameNumber()
        {
            return (byte)((this.bTileHeader2 & 48) >> 4);
        }

        public void frameNumber(byte frameNumber)
        {
            this.bTileHeader2 = (byte)((int)(this.bTileHeader2 & 207) | (int)(frameNumber & 3) << 4);
        }

        public byte wallFrameNumber()
        {
            return (byte)((this.bTileHeader2 & 192) >> 6);
        }

        public void wallFrameNumber(byte wallFrameNumber)
        {
            this.bTileHeader2 = (byte)((int)(this.bTileHeader2 & 63) | (int)(wallFrameNumber & 3) << 6);
        }

        public bool topSlope()
        {
            byte b = this.slope();
            return b == 1 || b == 2;
        }

        public bool bottomSlope()
        {
            byte b = this.slope();
            return b == 3 || b == 4;
        }

        public bool leftSlope()
        {
            byte b = this.slope();
            return b == 2 || b == 4;
        }

        public bool rightSlope()
        {
            byte b = this.slope();
            return b == 1 || b == 3;
        }

        public byte slope()
        {
            return (byte)((this.sTileHeader & 28672) >> 12);
        }

        public bool HasSameSlope(MemTile tile)
        {
            return (this.sTileHeader & 29696) == (tile.sTileHeader & 29696);
        }

        public void slope(byte slope)
        {
            this.sTileHeader = (short)(((int)this.sTileHeader & 36863) | (int)(slope & 7) << 12);
        }

        public int blockType()
        {
            if (this.halfBrick())
            {
                return 1;
            }
            int num = (int)this.slope();
            if (num > 0)
            {
                num++;
            }
            return num;
        }

        public byte color()
        {
            return (byte)(this.sTileHeader & 31);
        }

        public void color(byte color)
        {
            if (color > 30)
            {
                color = 30;
            }
            this.sTileHeader = (short)(((int)this.sTileHeader & 65504) | (int)color);
        }

        public byte wallColor()
        {
            return (byte)(this.bTileHeader & 31);
        }

        public void wallColor(byte wallColor)
        {
            if (wallColor > 30)
            {
                wallColor = 30;
            }
            this.bTileHeader = (byte)((this.bTileHeader & 224) | wallColor);
        }

        public bool lava()
        {
            return (this.bTileHeader & 32) == 32;
        }

        public void lava(bool lava)
        {
            if (lava)
            {
                this.bTileHeader = (byte)((this.bTileHeader & 159) | 32);
                return;
            }
            this.bTileHeader &= 223;
        }

        public bool honey()
        {
            return (this.bTileHeader & 64) == 64;
        }

        public void honey(bool honey)
        {
            if (honey)
            {
                this.bTileHeader = (byte)((this.bTileHeader & 159) | 64);
                return;
            }
            this.bTileHeader &= 191;
        }

        public void liquidType(int liquidType)
        {
            if (liquidType == 0)
            {
                this.bTileHeader &= 159;
                return;
            }
            if (liquidType == 1)
            {
                this.lava(true);
                return;
            }
            if (liquidType == 2)
            {
                this.honey(true);
            }
        }

        public byte liquidType()
        {
            return (byte)((this.bTileHeader & 96) >> 5);
        }

        public bool checkingLiquid()
        {
            return (this.bTileHeader3 & 8) == 8;
        }

        public void checkingLiquid(bool checkingLiquid)
        {
            if (checkingLiquid)
            {
                this.bTileHeader3 |= 8;
                return;
            }
            this.bTileHeader3 &= 247;
        }

        public bool skipLiquid()
        {
            return (this.bTileHeader3 & 16) == 16;
        }

        public void skipLiquid(bool skipLiquid)
        {
            if (skipLiquid)
            {
                this.bTileHeader3 |= 16;
                return;
            }
            this.bTileHeader3 &= 239;
        }

        public bool wire()
        {
            return (this.sTileHeader & 128) == 128;
        }

        public void wire(bool wire)
        {
            if (wire)
            {
                this.sTileHeader |= 128;
                return;
            }
            this.sTileHeader = (short)((int)this.sTileHeader & 65407);
        }

        public bool wire2()
        {
            return (this.sTileHeader & 256) == 256;
        }

        public void wire2(bool wire2)
        {
            if (wire2)
            {
                this.sTileHeader |= 256;
                return;
            }
            this.sTileHeader = (short)((int)this.sTileHeader & 65279);
        }

        public bool wire3()
        {
            return (this.sTileHeader & 512) == 512;
        }

        public void wire3(bool wire3)
        {
            if (wire3)
            {
                this.sTileHeader |= 512;
                return;
            }
            this.sTileHeader = (short)((int)this.sTileHeader & 65023);
        }

        public bool halfBrick()
        {
            return (this.sTileHeader & 1024) == 1024;
        }

        public void halfBrick(bool halfBrick)
        {
            if (halfBrick)
            {
                this.sTileHeader |= 1024;
                return;
            }
            this.sTileHeader = (short)((int)this.sTileHeader & 64511);
        }

        public bool actuator()
        {
            return (this.sTileHeader & 2048) == 2048;
        }

        public void actuator(bool actuator)
        {
            if (actuator)
            {
                this.sTileHeader |= 2048;
                return;
            }
            this.sTileHeader = (short)((int)this.sTileHeader & 63487);
        }

        public bool nactive()
        {
            int num = (int)(this.sTileHeader & 96);
            return num == 32;
        }

        public bool inActive()
        {
            return (this.sTileHeader & 64) == 64;
        }

        public void inActive(bool inActive)
        {
            if (inActive)
            {
                this.sTileHeader |= 64;
                return;
            }
            this.sTileHeader = (short)((int)this.sTileHeader & 65471);
        }

        public bool active()
        {
            return (this.sTileHeader & 32) == 32;
        }

        public void active(bool active)
        {
            if (active)
            {
                this.sTileHeader |= 32;
                return;
            }
            this.sTileHeader = (short)((int)this.sTileHeader & 65503);
        }

        public void ResetToType(ushort type)
        {
            this.liquid = 0;
            this.sTileHeader = 32;
            this.bTileHeader = 0;
            this.bTileHeader2 = 0;
            this.bTileHeader3 = 0;
            this.frameX = 0;
            this.frameY = 0;
            this.type = type;
        }

        public Color actColor(Color oldColor)
        {
            if (!this.inActive())
            {
                return oldColor;
            }
            double num = 0.4;
            return new Color((int)((byte)(num * (double)oldColor.R)), (int)((byte)(num * (double)oldColor.G)), (int)((byte)(num * (double)oldColor.B)), (int)oldColor.A);
        }

        public static void SmoothSlope(int x, int y, bool applyToNeighbors = true)
        {
            if (applyToNeighbors)
            {
                MemTile.SmoothSlope(x + 1, y, false);
                MemTile.SmoothSlope(x - 1, y, false);
                MemTile.SmoothSlope(x, y + 1, false);
                MemTile.SmoothSlope(x, y - 1, false);
            }

#if Full_API
            var tile = Terraria.Main.tile[x, y];
            if (!Terraria.WorldGen.SolidOrSlopedTile(x, y))
            {
                return;
            }
            bool flag = !Terraria.WorldGen.TileEmpty(x, y - 1);
            bool flag2 = !Terraria.WorldGen.SolidOrSlopedTile(x, y - 1) && flag;
            bool flag3 = Terraria.WorldGen.SolidOrSlopedTile(x, y + 1);
            bool flag4 = Terraria.WorldGen.SolidOrSlopedTile(x - 1, y);
            bool flag5 = Terraria.WorldGen.SolidOrSlopedTile(x + 1, y);
            switch ((flag ? 1 : 0) << 3 | (flag3 ? 1 : 0) << 2 | (flag4 ? 1 : 0) << 1 | (flag5 ? 1 : 0))
            {
                case 4:
                    tile.slope(0);
                    tile.halfBrick(true);
                    return;
                case 5:
                    tile.halfBrick(false);
                    tile.slope(2);
                    return;
                case 6:
                    tile.halfBrick(false);
                    tile.slope(1);
                    return;
                case 9:
                    if (!flag2)
                    {
                        tile.halfBrick(false);
                        tile.slope(4);
                        return;
                    }
                    return;
                case 10:
                    if (!flag2)
                    {
                        tile.halfBrick(false);
                        tile.slope(3);
                        return;
                    }
                    return;
            }
            tile.halfBrick(false);
            tile.slope(0);
#endif
        }

        public void ClearMetadata()
        {
            this.liquid = 0;
            this.sTileHeader = 0;
            this.bTileHeader = 0;
            this.bTileHeader2 = 0;
            this.bTileHeader3 = 0;
            this.frameX = 0;
            this.frameY = 0;
        }
    }
}
