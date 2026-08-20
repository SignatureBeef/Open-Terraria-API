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
#if TerrariaServer_1450_OrAbove || Terraria_1450_OrAbove || tModLoader_1450_OrAbove
    var transform = modder.GetILCursor(() => (new Terraria.NPC()).Transform(0, 0f, 0f, 0f, 0f, false));
#else
    var transform = modder.GetILCursor(() => (new Terraria.NPC()).Transform(0));
#endif

    transform.GotoNext(ins => ins.Operand is FieldReference fr && fr.Name == "netMode" && ins.Next.OpCode == OpCodes.Ldc_I4_2);
    transform.Emit(OpCodes.Ldarg_0);
    foreach(var parameter in transform.Method.Parameters)
        transform.Emit(OpCodes.Ldarga, parameter);
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

#if TerrariaServer_1450_OrAbove || Terraria_1450_OrAbove || tModLoader_1450_OrAbove
                public float Ai0 { get; set; }
                public float Ai1 { get; set; }
                public float Ai2 { get; set; }
                public float Ai3 { get; set; }
                public bool WithReposition { get; set; }
#endif
            }
            public static event EventHandler<TransformingEventArgs> Transforming;

            public static bool InvokeTransforming(Terraria.NPC instance, ref int newType
#if TerrariaServer_1450_OrAbove || Terraria_1450_OrAbove || tModLoader_1450_OrAbove
                , ref float ai0, ref float ai1, ref float ai2, ref float ai3, ref bool withReposition
#endif      
            )
            {
                var args = new TransformingEventArgs()
                {
                    Npc = instance,
                    NewType = newType,
#if TerrariaServer_1450_OrAbove || Terraria_1450_OrAbove || tModLoader_1450_OrAbove
                    Ai0 = ai0,
                    Ai1 = ai1,
                    Ai2 = ai2,
                    Ai3 = ai3,
                    WithReposition = withReposition,
#endif
                };
                Transforming?.Invoke(null, args);
                newType = args.NewType;
#if TerrariaServer_1450_OrAbove || Terraria_1450_OrAbove || tModLoader_1450_OrAbove
                ai0 = args.Ai0;
                ai1 = args.Ai1;
                ai2 = args.Ai2;
                ai3 = args.Ai3;
                withReposition = args.WithReposition;
#endif
                return args.Result != HookResult.Cancel;
            }
        }
    }
}
