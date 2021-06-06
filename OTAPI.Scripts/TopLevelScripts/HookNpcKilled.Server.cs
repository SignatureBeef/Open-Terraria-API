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
using System;
using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;

[Modification(ModType.PreMerge, "Hooking Terraria.NPC.checkDead")]
void HookNpcKilled(MonoModder modder)
{
    var checkDead = modder.GetILCursor(() => (new Terraria.NPC()).checkDead());

    checkDead.GotoNext(
        // active = false;
        i => i.OpCode == OpCodes.Ldc_I4_0
        , i => i.OpCode == OpCodes.Stfld && i.Operand is FieldReference fieldReference && fieldReference.Name == "active" && fieldReference.DeclaringType.FullName == "Terraria.Entity"
    );

    checkDead.EmitDelegate<NpcKilledCallback>(OTAPI.Callbacks.NPC.Killed);
    checkDead.Emit(OpCodes.Ldarg_0);
}

[MonoMod.MonoModIgnore]
public delegate void NpcKilledCallback(global::Terraria.NPC instance);

namespace OTAPI.Callbacks
{
    public static partial class NPC
    {
        public static void Killed(Terraria.NPC instance)
            => Hooks.NPC.InvokeKilled(new Hooks.NPC.KilledEventArgs()
            {
                npc = instance,
            });
    }
}

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class NPC
        {
            public class KilledEventArgs : EventArgs
            {
                public HookResult? Result { get; set; }

                public Terraria.NPC npc { get; set; }
            }
            public static event EventHandler<KilledEventArgs> Killed;

            public static HookResult? InvokeKilled(KilledEventArgs args)
            {
                Killed?.Invoke(null, args);
                return args.Result;
            }
        }
    }
}
