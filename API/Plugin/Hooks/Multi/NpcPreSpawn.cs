#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Npc can spawn data
        /// </summary>
        public struct NpcPreSpawn
        {
            public System.Boolean Sky { get; set; }

            public System.Boolean LihzahrdBrickWall { get; set; }

            public System.Boolean PlayerSafe { get; set; }

            public System.Boolean Invasion { get; set; }

            public System.Boolean Water { get; set; }

            public System.Boolean Granite { get; set; }

            public System.Boolean Marble { get; set; }

            public System.Boolean SpiderCave { get; set; }

            public  System.Boolean PlayerInTown { get; set; }

            public System.Boolean DesertCave { get; set; }

            public System.Boolean PlanteraDefeated { get; set; }

            public System.Boolean SafeRangeX { get; set; }

            public System.Int32 SpawnTileX { get; set; }

            public System.Int32 SpawnTileY { get; set; }

            public System.Int32 PlayerFloorX { get; set; }

            public System.Int32 PlayerFloorY{ get; set; }

            public override string ToString()
            {
                return string.Format("[NpcCanSpawn: Sky={0}, LihzahrdBrickWall={1}, PlayerSafe={2}, Invasion={3}, Water={4}, Granite={5}, Marble={6}, SpiderCave={7}, PlayerInTown={8}, DesertCave={9}, PlanteraDefeated={10}, SafeRangeX={11}, SpawnTileX={12}, SpawnTileY={13}, PlayerFloorX={14}, PlayerFloorY={15}]", Sky, LihzahrdBrickWall, PlayerSafe, Invasion, Water, Granite, Marble, SpiderCave, PlayerInTown, DesertCave, PlanteraDefeated, SafeRangeX, SpawnTileX, SpawnTileY, PlayerFloorX, PlayerFloorY);
            }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Occurs when NPC.SpawnNPC is about to spawn a new NPC
        /// </summary>
        public static readonly HookPoint<HookArgs.NpcPreSpawn> NpcPreSpawn = new HookPoint<HookArgs.NpcPreSpawn>();
    }
}
#endif