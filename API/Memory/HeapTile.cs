using System;

namespace OTA.Memory
{
    /// <summary>
    /// A tile implementation based on using the heap.
    /// </summary>
    /// <remarks>It's typically issued from a TileCollection instance.</remarks>
    public class HeapTile : MemTile
    {
        protected readonly int offset;
        protected byte[] heap;

        public const int kHeapTileSize = 13;

        public const int kHeapTileTypeOffset = 0;
        public const int kHeapTileWallOffset = 2;
        public const int kHeapTileLiquidOffset = 3;
        public const int kHeapTileSTileHeaderOffset = 4;
        public const int kHeapTileBTypeHeaderOffset = 6;
        public const int kHeapTileBTypeHeader2Offset = 7;
        public const int kHeapTileBTypeHeader3Offset = 8;
        public const int kHeapTileFrameXOffset = 9;
        public const int kHeapTileFrameYOffset = 11;

        protected int x;
        protected int y;

        public HeapTile(byte[] array, int x, int y)
        {
            heap = array;
            this.offset = (Terraria.Main.maxTilesY * x + y) * kHeapTileSize;
            this.x = x;
            this.y = y;
        }

        public override ushort _type
        {
            get
            {
                return (ushort)((heap[offset + kHeapTileTypeOffset + 1] << 8) | heap[offset + kHeapTileTypeOffset]);
            }

            set
            {
                heap[offset + kHeapTileTypeOffset + 1] = (byte)(value >> 8);
                heap[offset + kHeapTileTypeOffset] = (byte)(value & 0xFF);
            }
        }

        public override byte _wall
        {
            get
            {
                return heap[offset + kHeapTileWallOffset];
            }

            set
            {
                heap[offset + kHeapTileWallOffset] = value;
            }
        }

        public override byte _liquid
        {
            get
            {
                return heap[offset + kHeapTileLiquidOffset];
            }

            set
            {
                heap[offset + kHeapTileLiquidOffset] = value;
            }
        }

        public override short _sTileHeader
        {
            get
            {
                return (short)((heap[offset + kHeapTileSTileHeaderOffset + 1] << 8) | heap[offset + kHeapTileSTileHeaderOffset]);
            }

            set
            {
                heap[offset + kHeapTileSTileHeaderOffset + 1] = (byte)(value >> 8);
                heap[offset + kHeapTileSTileHeaderOffset] = (byte)(value & 0xFF);
            }
        }

        public override byte _bTileHeader
        {
            get
            {
                return heap[offset + kHeapTileBTypeHeaderOffset];
            }

            set
            {
                heap[offset + kHeapTileBTypeHeaderOffset] = value;
            }
        }

        public override byte _bTileHeader2
        {
            get
            {
                return heap[offset + kHeapTileBTypeHeader2Offset];
            }

            set
            {
                heap[offset + kHeapTileBTypeHeader2Offset] = value;
            }
        }

        public override byte _bTileHeader3
        {
            get
            {
                return heap[offset + kHeapTileBTypeHeader3Offset];
            }

            set
            {
                heap[offset + kHeapTileBTypeHeader3Offset] = value;
            }
        }

        public override short _frameX
        {
            get
            {
                return (short)((heap[offset + kHeapTileFrameXOffset + 1] << 8) | heap[offset + kHeapTileFrameXOffset]);
            }

            set
            {
                heap[offset + kHeapTileFrameXOffset + 1] = (byte)(value >> 8);
                heap[offset + kHeapTileFrameXOffset] = (byte)(value & 0xFF);
            }
        }

        public override short _frameY
        {
            get
            {
                return (short)((heap[offset + kHeapTileFrameYOffset + 1] << 8) | heap[offset + kHeapTileFrameYOffset]);
            }

            set
            {
                heap[offset + kHeapTileFrameYOffset + 1] = (byte)(value >> 8);
                heap[offset + kHeapTileFrameYOffset] = (byte)(value & 0xFF);
            }
        }
    }
}

