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
/// @doc Creates Hooks.NPC.Transform. Allows plugins to cancel NPC killed events.
/// </summary>
[Modification(ModType.PreMerge, "Hooking Npc.Transform")]
[MonoMod.MonoModIgnore]
void HookNpcTransform(MonoModder modder)
{
    var transform = modder.GetILCursor(() => (new Terraria.NPC()).Transform(0));

    transform.GotoNext(ins => ins.Operand is FieldReference fr && fr.Name == "netMode" && ins.Next.OpCode == OpCodes.Ldc_I4_2);
    transform.Emit(OpCodes.Ldarg_0);
    transform.Emit(OpCodes.Ldarga, transform.Method.Parameters.Single());
    transform.EmitDelegate(OTAPI.Hooks.NPC.InvokeTransforming);
    transform.Emit(OpCodes.Brtrue, transform.Next);
    transform.Emit(OpCodes.Ret);
}

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class NPC
        {
            public class TransformingEventArgs : EventArgs
            {
                public HookResult? Result { get; set; }

                public Terraria.NPC Npc { get; set; }
                public int NewType { get; set; }
            }
            public static event EventHandler<TransformingEventArgs> Transforming;

            public static bool InvokeTransforming(Terraria.NPC instance, ref int newType)
            {
                var args = new TransformingEventArgs()
                {
                    Npc = instance,
                    NewType = newType,
                };
                Transforming?.Invoke(null, args);
                newType = args.NewType;
                return args.Result != HookResult.Cancel;
            }
        }
    }
}
