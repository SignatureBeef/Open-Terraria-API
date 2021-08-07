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
using System.Linq;
using Terraria;

namespace OTAPI.Mods
{
    /// <summary>
    /// A class to create custom NPC's for Terraria. It should always be able to be called from .Net, lua or javascript.
    /// </summary>
    public class ModNPC : IMod
    {
        /// <summary>
        /// The vanilla NPC instance used by the game engine
        /// </summary>
        public NPC? NPC { get; set; }

        /// <summary>
        /// The name of your NPC mod
        /// </summary>
        public virtual string? Name { get; set; }

        /// <summary>
        /// The texture for your NPC
        /// </summary>
        public virtual string? Texture { get; set; }

        /// <summary>
        /// The type used by the vanilla game engine. This is managed by OTAPI but usually needed for mods if NPC.NewNPC is called manually.
        /// </summary>
        public int TypeID { get; private set; }

        /// <summary>
        /// Fired when your mod is registered with OTAPI. Not when it is being instanced.
        /// </summary>
        public event EventHandler? OnRegistered;

        /// <summary>
        /// Fired when your NPC is being created.
        /// </summary>
        public event EventHandler? OnCreated;

        public class SetDefaultsArgs : EventArgs
        {
            public HookResult Result { get; set; }
            public int Type { get; set; }
            public NPCSpawnParams SpawnParams { get; set; }
        }

        /// <summary>
        /// Fired when your NPC is being configured, after being created.
        /// </summary>
        public event EventHandler<SetDefaultsArgs>? OnSetDefaults;

        static ModNPC()
        {
            OTAPI.Hooks.NPC.Create += NPC_Create;
            On.Terraria.NPC.SetDefaults += NPC_SetDefaults;
        }

        private static void NPC_Create(object? sender, Hooks.NPC.CreateEventArgs e)
        {
            var mods = EntityDiscovery.GetTypeMods<ModNPC>();
            var mod = mods.SingleOrDefault(m => m.TypeID == e.Type);

            if (mod is not null)
            {
                var instance = (ModNPC)Activator.CreateInstance(mod.GetType())!;
                e.Npc = new NPC();
                e.Npc.EntityMod = instance;

                // copy rego to instance
                instance.TypeID = mod.TypeID;

                Console.WriteLine($"[OTAPI] Creating NPC instance: {e.Type}");

                mod.OnCreated?.Invoke(mod, EventArgs.Empty);
            }
        }

        private static void NPC_SetDefaults(On.Terraria.NPC.orig_SetDefaults orig, NPC self, int Type, NPCSpawnParams spawnparams)
        {
            if (self.EntityMod is ModNPC mod)
            {
                var args = new SetDefaultsArgs()
                {
                    Result = HookResult.Cancel, // assume the mod sets their own defaults, otherwise they can change the type and return continue. by default vanilla won't know what to do with the custom NPC type
                    Type = Type,
                    SpawnParams = spawnparams,
                };

                mod.OnSetDefaults?.Invoke(mod, args);

                if (args.Result == HookResult.Cancel)
                    return;

                Type = args.Type;
                spawnparams = args.SpawnParams;
            }

            orig(self, Type, spawnparams);
        }

        public void Registered()
        {
            // setup this type for instancing later on when the NPC needs to be spawned.
            TypeID = IModRegistry.AllocateType<ModNPC>(Terraria.Main.maxNPCTypes);

            Console.WriteLine($"[OTAPI] Assigned TypeID {TypeID} to {this.GetType().FullName}");

            OnRegistered?.Invoke(this, EventArgs.Empty);
        }
    }
}
