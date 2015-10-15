using System;

namespace OTA.Memory
{
    /// <summary>
    /// Terrarian tile data collection
    /// </summary>
    public class TileCollection
    {
        internal byte[] tileData;

        public TileCollection(int X, int Y)
        {
        }

        public MemTile this [int x, int y]
        {
            get
            {
                return GetTile(x, y);
            }
            set
            {
                SetTile(x, y, value);
            }
        }

        public unsafe MemTile GetTile(int x, int y)
        {
            if (tileData == null)
            {
                /*
                 * The array is created deliberately on the first getter, because Terraria
                 * has a bad habit of over-commiting the tile buffers when it starts up.
                 *
                 * -Wolfje
                 */
                int size = HeapTile.kHeapTileSize * (Terraria.Main.maxTilesX + 1) * (Terraria.Main.maxTilesY + 1);
                Logging.ProgramLog.Log("Creating tile array of {0}x{1}, {2}MB", Terraria.Main.maxTilesX,
                    Terraria.Main.maxTilesY, HeapTile.kHeapTileSize * Terraria.Main.maxTilesX * Terraria.Main.maxTilesY / 1024 / 1024);
                tileData = new byte[size];
            }
            return new HeapTile(tileData, x, y);
        }

        public unsafe void SetTile(int x, int y, MemTile tile)
        {
            HeapTile heapTile = new HeapTile(tileData, x, y);
            heapTile.CopyFrom(tile);
        }

        public static implicit operator TileCollection(MemTile[,] set)
        {
            return new TileCollection(set.GetLength(0), set.GetLength(1));
        }
    }
}

