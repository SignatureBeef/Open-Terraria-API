/*
Copyright (C) 2020 DeathCradle

This file is part of Open Terraria API v3 (OTAPI)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/
#pragma warning disable CS0436 // Type conflicts with imported type

using System;

namespace OTAPI.Mods
{
    public class TestNPC : ModNPC
    {
        public override Terraria.Localization.LocalizedText? Name { get; set; } = new Terraria.Localization.LocalizedText("TestNPC1", "Test 1");
        public override string? Texture { get; set; } = "TestNPC1.png";

        public TestNPC()
        {
            OnRegistered += Registered;
        }

        // inline test to add a secondary runtime NPC
        static bool inlinetest = false;

        // when the mod is registered, subscribe to when the player spawns into a world and see if we can spawn our test NPC
        // usually a mod would only attach to Created to use instance
        private void Registered(object? sender, EventArgs e)
        {
            On.Terraria.Player.Spawn += Player_Spawn;
            InlineTest();
        }

        private void Player_Spawn(On.Terraria.Player.orig_Spawn orig, Terraria.Player self, Terraria.PlayerSpawnContext context)
        {
            orig(self, context);

            Console.WriteLine($"[{GetName()}] Player spawned netMode {Terraria.Main.netMode}");
            Terraria.NPC.NewNPC(Terraria.Main.spawnTileX * 16, Terraria.Main.spawnTileY * 16, this.TypeID);
        }

        static void InlineTest()
        {
            if (!inlinetest)
            {
                inlinetest = true;

                var m = new ModNPC();
                m.Name = new Terraria.Localization.LocalizedText("TestNPC2", "Test 2");
                m.Texture = "TestNPC2.png";
                m.OnRegistered += (s, e) =>
                {
                    On.Terraria.Player.Spawn += (orig, self, context) =>
                    {
                        orig(self, context);

                        Console.WriteLine($"[{m.GetName()}] Player spawned netMode {Terraria.Main.netMode}");
                        Terraria.NPC.NewNPC(Terraria.Main.spawnTileX * 16, Terraria.Main.spawnTileY * 16, m.TypeID);
                    };
                };
                EntityDiscovery.Instance.AddEntityMod(m);
            }
        }
    }
}
