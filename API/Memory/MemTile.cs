//#define TESTING

using System;

namespace OTA.Memory
{
    /// <summary>
    /// Overridable terrarian tile. 
    /// [TODO] Allow use from SQL and so on, contact DeathCradle if you require 
    /// This is also possible to be done in the patcher/IL
    /// </summary>
    public class MemTile
    {
        #region Protected
        protected virtual byte _wall { get; set; }

        protected virtual byte _liquid { get; set; }

        protected virtual byte _bTileHeader { get; set; }

        protected virtual byte _bTileHeader2 { get; set; }

        protected virtual byte _bTileHeader3 { get; set; }

        protected virtual short _sTileHeader { get; set; }

        protected virtual short _frameX { get; set; }

        protected virtual short _frameY { get; set; }

        protected virtual ushort _type { get; set; }
        #endregion

        #region Properties
        public byte wall
        {
            get { return _wall; }
            set { _wall = value; }
        }

        public byte liquid
        {
            get { return _liquid; }
            set { _liquid = value; }
        }

        public byte bTileHeader
        {
            get { return _bTileHeader; }
            set { _bTileHeader = value; }
        }

        public byte bTileHeader2
        {
            get { return _bTileHeader2; }
            set { _bTileHeader2 = value; }
        }

        public byte bTileHeader3
        {
            get { return _bTileHeader3; }
            set { _bTileHeader3 = value; }
        }

        public short sTileHeader
        {
            get { return _sTileHeader; }
            set { _sTileHeader = value; }
        }

        public ushort type
        {
            get { return _type; }
            set { _type = value; }
        }

        public short frameX
        {
            get { return _frameX; }
            set { _frameX = value; }
        }

        public short frameY
        {
            get { return _frameY; }
            set { _frameY = value; }
        }
        #endregion

        #region Flags
        const Int32 Flag_WallColor = 31;
        const Int32 Flag_WallColor_Remove = byte.MaxValue - Flag_WallColor;

        const Int32 Flag_Lava = 32;
        const Int32 Flag_Lava_Remove = byte.MaxValue - Flag_Lava;

        const Int32 Flag_Honey = 64;
        const Int32 Flag_Honey_Remove = byte.MaxValue - Flag_Honey;

        const Int32 Flag_Wire4 = 128;
        const Int32 Flag_Wire4_Remove = byte.MaxValue - Flag_Wire4;

        const Int32 Flag_WallFrameX = 15;
        const Int32 Flag_WallFrameX_Remove = byte.MaxValue - Flag_WallFrameX;

        const Int32 Flag_FrameNumber = 48;
        const Int32 Flag_FrameNumber_Remove = byte.MaxValue - Flag_FrameNumber;

        const Int32 Flag_WallFrameNumber = 63;
        const Int32 Flag_WallFrameNumber_Remove = byte.MaxValue - Flag_WallFrameNumber;

        const Int32 Flag_WallFrameY = 7;
        const Int32 Flag_WallFrameY_Remove = byte.MaxValue - Flag_WallFrameY;

        const Int32 Flag_CheckingLiquid = 8;
        const Int32 Flag_CheckingLiquid_Remove = byte.MaxValue - Flag_CheckingLiquid;

        const Int32 Flag_SkipLiquid = 16;
        const Int32 Flag_SkipLiquid_Remove = byte.MaxValue - Flag_SkipLiquid;

        const Int32 Flag_Color = 31;
        const Int32 Flag_Color_Remove = ushort.MaxValue - Flag_Color;

        const Int32 Flag_Active = 32;
        const Int32 Flag_Active_Remove = ushort.MaxValue - Flag_Active;

        const Int32 Flag_Inactive = 64;
        const Int32 Flag_Inactive_Remove = ushort.MaxValue - Flag_Inactive;

        const Int32 Flag_Wire = 128;
        const Int32 Flag_Wire_Remove = ushort.MaxValue - Flag_Wire;

        const Int32 Flag_Wire2 = 256;
        const Int32 Flag_Wire2_Remove = ushort.MaxValue - Flag_Wire2;

        const Int32 Flag_Wire3 = 512;
        const Int32 Flag_Wire3_Remove = ushort.MaxValue - Flag_Wire3;

        const Int32 Flag_HalfBrick = 1024;
        const Int32 Flag_HalfBrick_Remove = ushort.MaxValue - Flag_HalfBrick;

        const Int32 Flag_Actuator = 2048;
        const Int32 Flag_Actuator_Remove = ushort.MaxValue - Flag_Actuator;
        #endregion

        public MemTile() { }

        public MemTile(MemTile copy)
        {
            if (copy == null)
            {
                type = 0;
                wall = 0;
                liquid = 0;
                sTileHeader = 0;
                bTileHeader = 0;
                bTileHeader2 = 0;
                bTileHeader3 = 0;
                frameX = 0;
                frameY = 0;
                return;
            }

            type = copy.type;
            wall = copy.wall;
            liquid = copy.liquid;
            sTileHeader = copy.sTileHeader;
            bTileHeader = copy.bTileHeader;
            bTileHeader2 = copy.bTileHeader2;
            bTileHeader3 = copy.bTileHeader3;
            frameX = copy.frameX;
            frameY = copy.frameY;
        }

        public int collisionType
        {
            get
            {
                if (!active())
                    return 0;

                if (halfBrick())
                    return 2;

                if (slope() > 0)
                    return (int)(2 + slope());

#if Full_API
                if (Terraria.Main.tileSolid[(int)type] && !Terraria.Main.tileSolidTop[(int)type])
                    return 1;
#endif
                return -1;
            }
        }

        public object Clone() => base.MemberwiseClone();

        public void ClearEverything()
        {
            type = 0;
            wall = 0;
            liquid = 0;
            sTileHeader = 0;
            bTileHeader = 0;
            bTileHeader2 = 0;
            bTileHeader3 = 0;
            frameX = 0;
            frameY = 0;
        }

        public void ClearTile()
        {
            slope(0);
            halfBrick(false);
            active(false);
        }

        public void CopyFrom(MemTile from)
        {
            type = from.type;
            wall = from.wall;
            liquid = from.liquid;
            sTileHeader = from.sTileHeader;
            bTileHeader = from.bTileHeader;
            bTileHeader2 = from.bTileHeader2;
            bTileHeader3 = from.bTileHeader3;
            frameX = from.frameX;
            frameY = from.frameY;
        }

        public bool isTheSameAs(MemTile compTile)
        {
            if (compTile == null)
                return false;

            if (sTileHeader != compTile.sTileHeader)
                return false;

            if (active())
            {
                if (type != compTile.type)
                    return false;

                if (Terraria.Main.tileFrameImportant[(int)type] && (frameX != compTile.frameX || frameY != compTile.frameY))
                    return false;
            }

            if (wall != compTile.wall || liquid != compTile.liquid)
                return false;

            if (compTile.liquid == 0)
            {
                if (wallColor() != compTile.wallColor())
                    return false;

                if (wire4() != compTile.wire4())
                    return false;
            }
            else if (bTileHeader != compTile.bTileHeader)
                return false;

            return true;
        }

        public int blockType()
        {
            if (halfBrick())
                return 1;

            var slope = (int)this.slope();
            if (slope > 0)
                slope++;

            return slope;
        }

        public void liquidType(int liquidType)
        {
            switch (liquidType)
            {
                case 0:
                    bTileHeader &= 159;
                    return;
                case 1:
                    lava(true);
                    return;
                case 2:
                    honey(true);
                    return;
            }
        }

        public byte liquidType() => (byte)((bTileHeader & 96) >> 5);

        public bool nactive() => (int)(sTileHeader & 96) == 32;

        public void ResetToType(ushort newType)
        {
            liquid = 0;
            sTileHeader = 32;
            bTileHeader = 0;
            bTileHeader2 = 0;
            bTileHeader3 = 0;
            frameX = 0;
            frameY = 0;
            type = newType;
        }

        public void ClearMetadata()
        {
            liquid = 0;
            sTileHeader = 0;
            bTileHeader = 0;
            bTileHeader2 = 0;
            bTileHeader3 = 0;
            frameX = 0;
            frameY = 0;
        }

        public Microsoft.Xna.Framework.Color actColor(Microsoft.Xna.Framework.Color oldColor)
        {
            if (!inActive()) return oldColor;

            var opacity = 0.4;
            return new Microsoft.Xna.Framework.Color
            (
                (int)((byte)(opacity * (double)oldColor.R)),
                (int)((byte)(opacity * (double)oldColor.G)),
                (int)((byte)(opacity * (double)oldColor.B)),
                (int)oldColor.A
            );
        }

        public bool topSlope()
        {
            var b = slope();
            return b == 1 || b == 2;
        }

        public bool bottomSlope()
        {
            var b = slope();
            return b == 3 || b == 4;
        }

        public bool leftSlope()
        {
            var b = slope();
            return b == 2 || b == 4;
        }

        public bool rightSlope()
        {
            var b = slope();
            return b == 1 || b == 3;
        }

        public bool HasSameSlope(MemTile tile) => (sTileHeader & 29696) == (tile.sTileHeader & 29696);

        public byte wallColor() => (byte)(bTileHeader & Flag_WallColor);

        public void wallColor(byte wallColor)
        {
            if (wallColor > 30)
                wallColor = 30;

            bTileHeader = (byte)((bTileHeader & Flag_WallColor_Remove) | wallColor);
        }

        public bool lava() => (bTileHeader & Flag_Lava) == Flag_Lava;

        public void lava(bool lava)
        {
            if (lava)
                bTileHeader = (byte)((bTileHeader & 159) | Flag_Lava);
            else bTileHeader &= Flag_Lava_Remove;
        }

        public bool honey() => (bTileHeader & Flag_Honey) == Flag_Honey;

        public void honey(bool honey)
        {
            if (honey)
                bTileHeader = (byte)((bTileHeader & 159) | Flag_Honey);
            else bTileHeader &= Flag_Honey_Remove;
        }

        public bool wire4() => (bTileHeader & Flag_Wire4) == Flag_Wire4;

        public void wire4(bool wire4)
        {
            if (wire4)
                bTileHeader |= Flag_Wire4;
            else bTileHeader &= Flag_Wire4_Remove;
        }

        public int wallFrameX() => (int)((bTileHeader2 & Flag_WallFrameX) * 36);

        public void wallFrameX(int wallFrameX)
        {
            bTileHeader2 = (byte)((int)(bTileHeader2 & Flag_WallFrameX_Remove) | (wallFrameX / 36 & 15));
        }

        public byte frameNumber() => (byte)((bTileHeader2 & Flag_FrameNumber) >> 4);

        public void frameNumber(byte frameNumber)
        {
            bTileHeader2 = (byte)((int)(bTileHeader2 & Flag_FrameNumber_Remove) | (int)(frameNumber & 3) << 4);
        }

        public byte wallFrameNumber() => (byte)((bTileHeader2 & Flag_WallFrameNumber_Remove) >> 6);

        public void wallFrameNumber(byte wallFrameNumber)
        {
            bTileHeader2 = (byte)((int)(bTileHeader2 & Flag_WallFrameNumber) | (int)(wallFrameNumber & 3) << 6);
        }

        public int wallFrameY() => (int)((bTileHeader3 & Flag_WallFrameY) * 36);

        public void wallFrameY(int wallFrameY)
        {
            bTileHeader3 = (byte)((int)(bTileHeader3 & Flag_WallFrameY_Remove) | (wallFrameY / 36 & Flag_WallFrameY));
        }

        public bool checkingLiquid() => (bTileHeader3 & Flag_CheckingLiquid) == Flag_CheckingLiquid;

        public void checkingLiquid(bool checkingLiquid)
        {
            if (checkingLiquid)
                bTileHeader3 |= Flag_CheckingLiquid;
            else bTileHeader3 &= Flag_CheckingLiquid_Remove;
        }

        public bool skipLiquid() => (bTileHeader3 & Flag_SkipLiquid) == Flag_SkipLiquid;

        public void skipLiquid(bool skipLiquid)
        {
            if (skipLiquid)
                bTileHeader3 |= Flag_SkipLiquid;
            else bTileHeader3 &= Flag_SkipLiquid_Remove;
        }

        public byte color() => (byte)(sTileHeader & Flag_Color);

        public void color(byte color)
        {
            if (color > 30)
                color = 30;

            sTileHeader = (short)(((int)sTileHeader & Flag_Color_Remove) | (int)color);
        }

        public bool active() => (sTileHeader & Flag_Active) == Flag_Active;

        public void active(bool active)
        {
            if (active)
                sTileHeader |= Flag_Active;
            else sTileHeader = (short)((int)sTileHeader & Flag_Active_Remove);
        }

        public bool inActive() => (sTileHeader & Flag_Inactive) == Flag_Inactive;

        public void inActive(bool inActive)
        {
            if (inActive)
                sTileHeader |= Flag_Inactive;
            else sTileHeader = (short)((int)sTileHeader & Flag_Inactive_Remove);
        }

        public bool wire() => (sTileHeader & Flag_Wire) == Flag_Wire;

        public void wire(bool wire)
        {
            if (wire)
            {
                sTileHeader |= Flag_Wire;
                return;
            }
            sTileHeader = (short)((int)sTileHeader & Flag_Wire_Remove);
        }

        public bool wire2() => (sTileHeader & Flag_Wire2) == Flag_Wire2;

        public void wire2(bool wire2)
        {
            if (wire2)
                sTileHeader |= Flag_Wire2;
            else sTileHeader = (short)((int)sTileHeader & Flag_Wire2_Remove);
        }

        public bool wire3() => (sTileHeader & Flag_Wire3) == Flag_Wire3;

        public void wire3(bool wire3)
        {
            if (wire3)
                sTileHeader |= Flag_Wire3;
            else sTileHeader = (short)((int)sTileHeader & Flag_Wire3_Remove);
        }

        public bool halfBrick() => (sTileHeader & Flag_HalfBrick) == Flag_HalfBrick;

        public void halfBrick(bool halfBrick)
        {
            if (halfBrick)
                sTileHeader |= Flag_HalfBrick;
            else sTileHeader = (short)((int)sTileHeader & Flag_HalfBrick_Remove);
        }

        public bool actuator() => (sTileHeader & Flag_Actuator) == Flag_Actuator;

        public void actuator(bool actuator)
        {
            if (actuator)
                sTileHeader |= Flag_Actuator;
            else sTileHeader = (short)((int)sTileHeader & Flag_Actuator_Remove);
        }

        public byte slope() => (byte)((sTileHeader & 28672) >> 12);

        public void slope(byte slope)
        {
            sTileHeader = (short)(((int)sTileHeader & 36863) | (int)(slope & 7) << 12);
        }

        public static void SmoothSlope(int x, int y, bool applyToNeighbors = true)
        {
            if (applyToNeighbors)
            {
                SmoothSlope(x + 1, y, false);
                SmoothSlope(x - 1, y, false);
                SmoothSlope(x, y + 1, false);
                SmoothSlope(x, y - 1, false);
            }

            var tile = Terraria.Main.tile[x, y];
            if (!Terraria.WorldGen.SolidOrSlopedTile(x, y))
                return;

            var topSet = !Terraria.WorldGen.TileEmpty(x, y - 1);
            var top = !Terraria.WorldGen.SolidOrSlopedTile(x, y - 1) && topSet;
            var bottom = Terraria.WorldGen.SolidOrSlopedTile(x, y + 1);
            var left = Terraria.WorldGen.SolidOrSlopedTile(x - 1, y);
            var right = Terraria.WorldGen.SolidOrSlopedTile(x + 1, y);

            switch ((topSet ? 1 : 0) << 3 | (bottom ? 1 : 0) << 2 | (left ? 1 : 0) << 1 | (right ? 1 : 0))
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
                    if (!top)
                    {
                        tile.halfBrick(false);
                        tile.slope(4);
                        return;
                    }
                    return;
                case 10:
                    if (!top)
                    {
                        tile.halfBrick(false);
                        tile.slope(3);
                        return;
                    }
                    return;
            }
            tile.halfBrick(false);
            tile.slope(0);
        }

        public override string ToString() => $"Tile Type:{type} Active:{active()} Wall:{wall} Slope:{slope()} fX:{frameX} fY:{frameY}";
    }
}
