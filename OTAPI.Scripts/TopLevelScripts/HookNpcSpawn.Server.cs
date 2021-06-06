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
using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;
using System;
using System.Linq;

[Modification(ModType.PreMerge, "Hooking Terraria.NPC.NewNPC(Spawn)")]
void HookNpcSpawn(MonoModder modder)
{
    var NewNPC = modder.GetILCursor(() => Terraria.NPC.NewNPC(default, default, default, default, default, default, default, default, default));

    NewNPC.GotoNext(
        i => i.OpCode == OpCodes.Stfld && i.Operand is FieldReference fieldReference && fieldReference.Name == "target" && fieldReference.DeclaringType.FullName == "Terraria.NPC"
    );
    NewNPC.Index++;

    NewNPC.Emit(OpCodes.Ldloca, NewNPC.Body.Variables.First());
    NewNPC.EmitDelegate<NpcSpawnCallback>(OTAPI.Callbacks.NPC.Spawn);
    NewNPC.Emit(OpCodes.Brtrue_S, NewNPC.Instrs[NewNPC.Index]);
    NewNPC.Emit(OpCodes.Ldloc, NewNPC.Body.Variables.First());
    NewNPC.Emit(OpCodes.Ret);
}

[MonoMod.MonoModIgnore]
public delegate bool NpcSpawnCallback(ref int index);

namespace OTAPI.Callbacks
{
    public static partial class NPC
    {
        public static bool Spawn(ref int index)
        {
            var args = new Hooks.NPC.SpawnEventArgs()
            {
                index = index,
            };
            var result = Hooks.NPC.InvokeSpawn(args);

            index = args.index;

            return result != HookResult.Cancel;
        }
    }
}

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class NPC
        {
            public class SpawnEventArgs : EventArgs
            {
                public HookResult? Result { get; set; }

                public int index { get; set; }
            }
            public static event EventHandler<SpawnEventArgs> Spawn;

            public static HookResult? InvokeSpawn(SpawnEventArgs args)
            {
                Spawn?.Invoke(null, args);
                return args.Result;
            }
        }
    }
}

