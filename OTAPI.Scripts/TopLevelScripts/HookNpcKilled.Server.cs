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

using System;
using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;

/// <summary>
/// @doc Creates Hooks.NPC.Killed. Allows plugins to cancel NPC killed events.
/// </summary>
[Modification(ModType.PreMerge, "Hooking Terraria.NPC.checkDead")]
[MonoMod.MonoModIgnore]
void HookNpcKilled(MonoModder modder)
{
    var checkDead = modder.GetILCursor(() => (new Terraria.NPC()).checkDead());

    checkDead.GotoNext(
        // active = false;
        i => i.OpCode == OpCodes.Ldc_I4_0
        , i => i.OpCode == OpCodes.Stfld && i.Operand is FieldReference fieldReference && fieldReference.Name == "active" && fieldReference.DeclaringType.FullName == "Terraria.Entity"
    );

    checkDead.EmitDelegate<NpcKilledCallback>(OTAPI.Hooks.NPC.InvokeKilled);
    checkDead.Emit(OpCodes.Ldarg_0);
}

[MonoMod.MonoModIgnore]
public delegate void NpcKilledCallback(global::Terraria.NPC instance);


namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class NPC
        {
            public class KilledEventArgs : EventArgs
            {
                public Terraria.NPC Npc { get; set; }
            }
            public static event EventHandler<KilledEventArgs> Killed;

            public static void InvokeKilled(Terraria.NPC instance)
            {
                var args = new KilledEventArgs()
                {
                    Npc = instance,
                };
                Killed?.Invoke(null, args);
            }
        }
    }
}
