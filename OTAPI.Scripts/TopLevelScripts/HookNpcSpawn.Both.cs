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
#pragma warning disable CS8321 // Local function is declared but never used
#pragma warning disable CS0436 // Type conflicts with imported type

using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;
using System;
using System.Linq;

/// <summary>
/// @doc Creates Hooks.NPC.Spawn. Allows plugins to cancel NPC spawns.
/// </summary>
[Modification(ModType.PreMerge, "Hooking Terraria.NPC.NewNPC(Spawn)")]
void HookNpcSpawn(MonoModder modder)
{
    var NewNPC = modder.GetILCursor(() => Terraria.NPC.NewNPC(default, default, default, default, default, default, default, default, default));

    NewNPC.GotoNext(
        i => i.OpCode == OpCodes.Stfld && i.Operand is FieldReference fieldReference && fieldReference.Name == "target" && fieldReference.DeclaringType.FullName == "Terraria.NPC"
    );
    NewNPC.Index++;

    NewNPC.Emit(OpCodes.Ldloca, NewNPC.Body.Variables.First());
    NewNPC.EmitDelegate<NpcSpawnCallback>(OTAPI.Hooks.NPC.InvokeSpawn);
    NewNPC.Emit(OpCodes.Brtrue_S, NewNPC.Instrs[NewNPC.Index]);
    NewNPC.Emit(OpCodes.Ldloc, NewNPC.Body.Variables.First());
    NewNPC.Emit(OpCodes.Ret);
}

[MonoMod.MonoModIgnore]
public delegate bool NpcSpawnCallback(ref int index);

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class NPC
        {
            public class SpawnEventArgs : EventArgs
            {
                public HookResult? Result { get; set; }

                public int Index { get; set; }
            }
            public static event EventHandler<SpawnEventArgs> Spawn;

            public static bool InvokeSpawn(ref int index)
            {
                var args = new SpawnEventArgs()
                {
                    Index = index,
                };
                Spawn?.Invoke(null, args);

                index = args.Index;

                return args.Result != HookResult.Cancel;
            }
        }
    }
}

