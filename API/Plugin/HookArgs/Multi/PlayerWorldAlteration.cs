#if CLIENT || SERVER
using System;
using OTA.ID;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct PlayerWorldAlteration
        {
            public int X { get; set; }

            public int Y { get; set; }
            public ActionType Action { get; set; }

            public short Type { get; set; }

            public int Style { get; set; }

            public bool TypeChecked { get; set; }

            //            public WorldMod.PlayerSandbox Sandbox { get; internal set; }

            public bool TileWasRemoved => Action == ActionType.KillTile || Action == ActionType.KillTile1 || Action == ActionType.UNKNOWN_1;

            public bool NoItem
            {
                get { return Action == ActionType.KillTile1 || Action == ActionType.UNKNOWN_2; }
                set
                {
                    if (value)
                    {
                        if (Action == ActionType.KillTile)
                            Action = ActionType.KillTile1;
                        else if (Action == ActionType.UNKNOWN_1)
                            Action = ActionType.UNKNOWN_2;
                    }
                    else
                    {
                        if (Action == ActionType.KillTile1)
                            Action = ActionType.KillTile;
                        else if (Action == ActionType.UNKNOWN_2)
                            Action = ActionType.UNKNOWN_1;
                    }
                }
            }

            public bool TileWasPlaced => Action == ActionType.PlaceTile;

            public bool WallWasRemoved => Action == ActionType.KillWall || Action == ActionType.UNKNOWN_1 || Action == ActionType.UNKNOWN_2;

            public bool WallWasPlaced => Action == ActionType.PlaceWall;

            public bool RemovalFailed
            {
                get { return Type == 1 && (Action == ActionType.KillTile || Action == ActionType.PlaceTile || Action == ActionType.KillTile1); }
                set
                {
                    if (Action == (int)ActionType.KillTile || Action == ActionType.KillWall || Action == ActionType.KillTile1)
                        Type = value ? (byte)1 : (byte)0;
                }
            }

#if Full_API && !MemTile && !VANILLACOMPAT
public Terraria.Tile Tile => Main.tile[X, Y];
#elif Full_API && (MemTile || VANILLACOMPAT) && TileReady
            public OTA.Memory.MemTile Tile => Terraria.Main.tile[X, Y];
#endif
        }
    }
}
#endif