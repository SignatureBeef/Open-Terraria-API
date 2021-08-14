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
using Terraria;
using Terraria.ID;

namespace OTAPI.Mods
{
    public class TestNPC : ModNPC
    {
        public override Terraria.Localization.LocalizedText? Name { get; set; } = new Terraria.Localization.LocalizedText("TestNPC1", "Test 1");
        public override string? Texture { get; set; } = "TestNPC1";

        public TestNPC()
        {
            OnRegistered += Registered;
            OnSetDefaults += TestNPC_OnSetDefaults;
        }

        private void TestNPC_OnSetDefaults(object? sender, EventArgs e)
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = 7;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;

            Main.npcCatchable[TypeID] = false;
            Main.npcFrameCount[TypeID] = 16;
            NPC.killCount[TypeID] = 0;
            Main.townNPCCanSpawn[TypeID] = false;
            Main.slimeRainNPC[TypeID] = false;
            Terraria.GameContent.UI.EmoteBubble.CountNPCs[TypeID] = 0;
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
            return;
            if (!inlinetest)
            {
                inlinetest = true;

                var m = new ModNPC();
                m.Name = new Terraria.Localization.LocalizedText("TestNPC2", "Test 2");
                m.Texture = "TestNPC2";
                m.OnRegistered += (s, e) =>
                {
                    On.Terraria.Player.Spawn += (orig, self, context) =>
                    {
                        orig(self, context);

                        Console.WriteLine($"[{m.GetName()}] Player spawned netMode {Terraria.Main.netMode}");
                        Terraria.NPC.NewNPC(Terraria.Main.spawnTileX * 16, Terraria.Main.spawnTileY * 16, m.TypeID);
                    };
                };
                m.OnSetDefaults += (s, e) =>
                {
                    m.NPC.townNPC = true;
                    m.NPC.friendly = true;
                    m.NPC.width = 18;
                    m.NPC.height = 40;
                    m.NPC.aiStyle = 7;
                    m.NPC.damage = 10;
                    m.NPC.defense = 15;
                    m.NPC.lifeMax = 250;
                    m.NPC.HitSound = SoundID.NPCHit1;
                    m.NPC.DeathSound = SoundID.NPCDeath1;
                    m.NPC.knockBackResist = 0.5f;

                    Main.npcCatchable[m.TypeID] = false;
                    Main.npcFrameCount[m.TypeID] = 16;
                    NPC.killCount[m.TypeID] = 0;
                    Main.townNPCCanSpawn[m.TypeID] = false;
                    Main.slimeRainNPC[m.TypeID] = false;
                    Terraria.GameContent.UI.EmoteBubble.CountNPCs[m.TypeID] = 0;
                };
                EntityDiscovery.Instance.AddEntityMod(m);
            }
        }
    }
}
