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

using Terraria;

namespace OTAPI.Mods
{
    /// <summary>
    /// A class to create custom NPC's for Terraria. It should always be able to be called from .Net, lua or javascript.
    /// </summary>
    public class ModNPC : IMod
    {
        public NPC? NPC { get; set; }

        public virtual string? Name { get; set; }
        public virtual string? Texture { get; set; }

        public virtual bool OnSetDefaults(ref int type, ref NPCSpawnParams spawnparams) => false;

        static ModNPC()
        {
            On.Terraria.NPC.SetDefaults += NPC_SetDefaults;
            On.Terraria.NPC.NewNPC += NPC_NewNPC;
            //new Hook(typeof(NPC).GetMethod("NewNPC"), new Func<Func<int, int, int, int, float, float, float, float, int, int>, int, int, int, int, float, float, float, float, int, int>(NPC_NewNPC));
            //new Hook(typeof(NPC).GetMethod("SetDefaults"), new Action<Action<NPC, int, NPCSpawnParams>, NPC, int, NPCSpawnParams>(NPC_SetDefaults));
        }

        //private static int NPC_NewNPC(Func<int, int, int, int, float, float, float, float, int, int> orig, int X, int Y, int Type, int Start, float ai0, float ai1, float ai2, float ai3, int Target)
        private static int NPC_NewNPC(On.Terraria.NPC.orig_NewNPC orig, int X, int Y, int Type, int Start, float ai0, float ai1, float ai2, float ai3, int Target)
        {
            var mods = EntityDiscovery.GetTypeMods<ModNPC>();
            //var match = mods.Where(m => m.Attribute.Name == );

            return orig(X, Y, Type, Start, ai0, ai1, ai2, ai3, Target);
        }

        //private static void NPC_SetDefaults(Action<NPC, int, NPCSpawnParams> orig, NPC self, int Type, NPCSpawnParams spawnparams)
        private static void NPC_SetDefaults(On.Terraria.NPC.orig_SetDefaults orig, NPC self, int Type, NPCSpawnParams spawnparams)
        {
            if (self.EntityMod is ModNPC mod && !mod.OnSetDefaults(ref Type, ref spawnparams))
                return;

            orig(self, Type, spawnparams);
        }
    }
}
