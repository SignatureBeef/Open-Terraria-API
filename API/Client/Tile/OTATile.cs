#if CLIENT
using System;
using OTA.Plugin;
using Terraria;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace OTA.Client.Tile
{
    public abstract class OTATile : INativeMod
    {
        public ushort TypeId { get; set; }

        public void EmulateTile(int cloneFromTypeId)
        {
            if (!Terraria.Main.tileSetsLoaded[cloneFromTypeId])
            {
                Terraria.Main.instance.LoadTiles(cloneFromTypeId);
            }

            var newTypeId = TypeId;
            Terraria.Main.tileAlch[newTypeId] = Terraria.Main.tileAlch[cloneFromTypeId];
            Terraria.Main.tileAxe[newTypeId] = Terraria.Main.tileAxe[cloneFromTypeId];
            Terraria.Main.tileBlendAll[newTypeId] = Terraria.Main.tileBlendAll[cloneFromTypeId];
            Terraria.Main.tileBlockLight[newTypeId] = Terraria.Main.tileBlockLight[cloneFromTypeId];
            Terraria.Main.tileBouncy[newTypeId] = Terraria.Main.tileBouncy[cloneFromTypeId];
            Terraria.Main.tileBrick[newTypeId] = Terraria.Main.tileBrick[cloneFromTypeId];
            Terraria.Main.tileContainer[newTypeId] = Terraria.Main.tileContainer[cloneFromTypeId];
            Terraria.Main.tileCut[newTypeId] = Terraria.Main.tileCut[cloneFromTypeId];
            Terraria.Main.tileDungeon[newTypeId] = Terraria.Main.tileDungeon[cloneFromTypeId];
            Terraria.Main.tileFlame[newTypeId] = Terraria.Main.tileFlame[cloneFromTypeId];
            Terraria.Main.tileFrame[newTypeId] = Terraria.Main.tileFrame[cloneFromTypeId];
            Terraria.Main.tileFrameCounter[newTypeId] = Terraria.Main.tileFrameCounter[cloneFromTypeId];
            Terraria.Main.tileFrameImportant[newTypeId] = Terraria.Main.tileFrameImportant[cloneFromTypeId];
            Terraria.Main.tileGlowMask[newTypeId] = Terraria.Main.tileGlowMask[cloneFromTypeId];
            Terraria.Main.tileHammer[newTypeId] = Terraria.Main.tileHammer[cloneFromTypeId];
            Terraria.Main.tileLargeFrames[newTypeId] = Terraria.Main.tileLargeFrames[cloneFromTypeId];
            Terraria.Main.tileLavaDeath[newTypeId] = Terraria.Main.tileLavaDeath[cloneFromTypeId];
            Terraria.Main.tileLighted[newTypeId] = Terraria.Main.tileLighted[cloneFromTypeId];
            Terraria.Main.tileMergeDirt[newTypeId] = Terraria.Main.tileMergeDirt[cloneFromTypeId];
            Terraria.Main.tileMoss[newTypeId] = Terraria.Main.tileMoss[cloneFromTypeId];
            Terraria.Main.tileNoAttach[newTypeId] = Terraria.Main.tileNoAttach[cloneFromTypeId];
            Terraria.Main.tileNoFail[newTypeId] = Terraria.Main.tileNoFail[cloneFromTypeId];
            Terraria.Main.tileNoSunLight[newTypeId] = Terraria.Main.tileNoSunLight[cloneFromTypeId];
            Terraria.Main.tileObsidianKill[newTypeId] = Terraria.Main.tileObsidianKill[cloneFromTypeId];
            Terraria.Main.tilePile[newTypeId] = Terraria.Main.tilePile[cloneFromTypeId];
            Terraria.Main.tileRope[newTypeId] = Terraria.Main.tileRope[cloneFromTypeId];
            Terraria.Main.tileSand[newTypeId] = Terraria.Main.tileSand[cloneFromTypeId];
            Terraria.Main.tileSetsLoaded[newTypeId] = Terraria.Main.tileSetsLoaded[cloneFromTypeId];
            Terraria.Main.tileShine[newTypeId] = Terraria.Main.tileShine[cloneFromTypeId];
            Terraria.Main.tileShine2[newTypeId] = Terraria.Main.tileShine2[cloneFromTypeId];
            Terraria.Main.tileSolid[newTypeId] = Terraria.Main.tileSolid[cloneFromTypeId];
            Terraria.Main.tileSolidTop[newTypeId] = Terraria.Main.tileSolidTop[cloneFromTypeId];
            Terraria.Main.tileSpelunker[newTypeId] = Terraria.Main.tileSpelunker[cloneFromTypeId];
            Terraria.Main.tileStone[newTypeId] = Terraria.Main.tileStone[cloneFromTypeId];
            Terraria.Main.tileTable[newTypeId] = Terraria.Main.tileTable[cloneFromTypeId];
            Terraria.Main.tileTexture[newTypeId] = Terraria.Main.tileTexture[cloneFromTypeId];
            Terraria.Main.tileValue[newTypeId] = Terraria.Main.tileValue[cloneFromTypeId];
            Terraria.Main.tileWaterDeath[newTypeId] = Terraria.Main.tileWaterDeath[cloneFromTypeId];

            Terraria.WorldGen.tileCounts[newTypeId] = Terraria.WorldGen.tileCounts[cloneFromTypeId];

            Terraria.Map.MapHelper.colorLookup[newTypeId] = Terraria.Map.MapHelper.colorLookup[cloneFromTypeId];
            Terraria.Map.MapHelper.tileLookup[newTypeId] = Terraria.Map.MapHelper.tileLookup[cloneFromTypeId];
            Terraria.Map.MapHelper.tileOptionCounts[newTypeId] = Terraria.Map.MapHelper.tileOptionCounts[cloneFromTypeId];
        }

        internal void Initialise()
        {
            OnSetDefaults();
        }

        public OTATile()
        {

        }

        public virtual void OnSetDefaults()
        {
        }

        #region "Handlers"

        static void Resize<T>(ref T[] array, int length)
        {
            if (array.Length != length)
            {
                if (array.Length < length)
                {
                    Array.Resize(ref array, length);
                }
                else
                {
                    Console.WriteLine("Array was being decreased when it should not");
                }
            }
        }

        internal static void ResizeArrays(bool force = false, bool append = true)
        {
            //This is an expensive method so we are issuing blocks of space
            const Int32 BlockIssueSize = 50;

            if (force || TileModRegister.MaxId + 1 >= Terraria.ID.TileID.Sets.AllTiles.Length)
            {
                var length = Terraria.ID.TileID.Sets.AllTiles.Length + (append ? BlockIssueSize : 0);

                Resize(ref Terraria.Main.tileAlch, length);
                Resize(ref Terraria.Main.tileAxe, length);
                Resize(ref Terraria.Main.tileBlendAll, length);
                Resize(ref Terraria.Main.tileBlockLight, length);
                Resize(ref Terraria.Main.tileBouncy, length);
                Resize(ref Terraria.Main.tileBrick, length);
                Resize(ref Terraria.Main.tileContainer, length);
                Resize(ref Terraria.Main.tileCut, length);
                Resize(ref Terraria.Main.tileDungeon, length);
                Resize(ref Terraria.Main.tileFlame, length);
                Resize(ref Terraria.Main.tileFrame, length);
                Resize(ref Terraria.Main.tileFrameCounter, length);
                Resize(ref Terraria.Main.tileFrameImportant, length);
                Resize(ref Terraria.Main.tileGlowMask, length);
                Resize(ref Terraria.Main.tileHammer, length);
                Resize(ref Terraria.Main.tileLargeFrames, length);
                Resize(ref Terraria.Main.tileLavaDeath, length);
                Resize(ref Terraria.Main.tileLighted, length);
                Resize(ref Terraria.Main.tileMergeDirt, length);
                Resize(ref Terraria.Main.tileMoss, length);
                Resize(ref Terraria.Main.tileNoAttach, length);
                Resize(ref Terraria.Main.tileNoFail, length);
                Resize(ref Terraria.Main.tileNoSunLight, length);
                Resize(ref Terraria.Main.tileObsidianKill, length);
                Resize(ref Terraria.Main.tilePile, length);
                Resize(ref Terraria.Main.tileRope, length);
                Resize(ref Terraria.Main.tileSand, length);
                Resize(ref Terraria.Main.tileSetsLoaded, length);
                Resize(ref Terraria.Main.tileShine, length);
                Resize(ref Terraria.Main.tileShine2, length);
                Resize(ref Terraria.Main.tileSolid, length);
                Resize(ref Terraria.Main.tileSolidTop, length);
                Resize(ref Terraria.Main.tileSpelunker, length);
                Resize(ref Terraria.Main.tileStone, length);
                Resize(ref Terraria.Main.tileTable, length);
                Resize(ref Terraria.Main.tileTexture, length);
                Resize(ref Terraria.Main.tileValue, length);
                Resize(ref Terraria.Main.tileWaterDeath, length);

                Resize(ref Terraria.WorldGen.tileCounts, length);

                if (null == Terraria.Map.MapHelper.colorLookup) Terraria.Map.MapHelper.colorLookup = new Microsoft.Xna.Framework.Color[length];
                else Resize(ref Terraria.Map.MapHelper.colorLookup, length);

                if (null == Terraria.Map.MapHelper.tileLookup) Terraria.Map.MapHelper.tileLookup = new ushort[length];
                else Resize(ref Terraria.Map.MapHelper.tileLookup, length);

                if (null == Terraria.Map.MapHelper.tileOptionCounts) Terraria.Map.MapHelper.tileOptionCounts = new int[length];
                else Resize(ref Terraria.Map.MapHelper.tileOptionCounts, length);

                Console.WriteLine("New tile size: " + length);

                //Resize ID sets
                foreach (var field in typeof(Terraria.ID.TileID.Sets).GetFields())
                {
                    if (field.FieldType.IsArray)
                    {
                        var arr = field.GetValue(null) as Array;
                        var t = field.FieldType.GetElementType();

                        var args = new object[] { arr, length };
                        typeof(Array).GetMethod("Resize").MakeGenericMethod(t).Invoke(null, args);

                        field.SetValue(null, args[0]);
                    }
                }
            }
        }

        #endregion
    }
}
#endif