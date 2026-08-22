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
using MonoMod.Cil;
using System;
using Terraria.DataStructures;

/// <summary>
/// @doc Creates Hooks.NPC.Create. Allows plugins to create NPC instances.
/// </summary>
[Modification(ModType.PreMerge, "Hooking Terraria.NPC.NewNPC(Create)")]
[MonoMod.MonoModIgnore]
void HookNpcCreate(MonoModder modder)
{
#if Terraria_1457_OrAbove || TerrariaServer_1457_OrAbove
    var callback = modder.GetMethodDefinition(() => OTAPI.Hooks.NPC.InvokeCreate(default, default, default, default, default, default, default, default, default, default, default, default, default));
    var NewNPC = modder.GetILCursor(() => Terraria.NPC.NewNPC(default, default, default, default, default, default, default, default, default, default));
#elif TerrariaServer_EntitySourcesActive || Terraria_EntitySourcesActive || tModLoader_EntitySourcesActive
    var callback = modder.GetMethodDefinition(() => OTAPI.Hooks.NPC.InvokeCreate(default, default, default, default, default, default, default, default, default, default));
    var NewNPC = modder.GetILCursor(() => Terraria.NPC.NewNPC(default, default, default, default, default, default, default, default, default, default));
#else
    var callback = modder.GetMethodDefinition(() => OTAPI.Hooks.NPC.InvokeCreate(default, default, default, default, default, default, default, default, default));
    var NewNPC = modder.GetILCursor(() => Terraria.NPC.NewNPC(default, default, default, default, default, default, default, default, default));
#endif

    NewNPC.GotoNext(
#if Terraria_1457_OrAbove || TerrariaServer_1457_OrAbove
        i => i.OpCode == OpCodes.Call && i.Operand is MethodReference mr && mr.Name == "NewNPCInstanceInSlot"
#else
        i => i.OpCode == OpCodes.Newobj && i.Operand is MethodReference mr && mr.Name == ".ctor" && mr.DeclaringType.FullName == "Terraria.NPC"
#endif
    );

#if Terraria_1457_OrAbove || TerrariaServer_1457_OrAbove
    NewNPC.GotoNext(MoveType.After,
        i => i.OpCode == OpCodes.Stloc_1 ||
             (i.OpCode == OpCodes.Stloc && i.Operand is VariableDefinition vd && vd.Index == 1)
    );

    NewNPC.Emit(OpCodes.Ldloc_0);         
    NewNPC.Emit(OpCodes.Ldc_I4_0);        
    NewNPC.Emit(OpCodes.Ldarg_0);         
    NewNPC.Emit(OpCodes.Ldarg_1);         
    NewNPC.Emit(OpCodes.Ldarg_2);         
    NewNPC.Emit(OpCodes.Ldarg_3);         
    NewNPC.Emit(OpCodes.Ldarg_S, (byte)4);
    NewNPC.Emit(OpCodes.Ldarg_S, (byte)5);
    NewNPC.Emit(OpCodes.Ldarg_S, (byte)6);
    NewNPC.Emit(OpCodes.Ldarg_S, (byte)7);
    NewNPC.Emit(OpCodes.Ldarg_S, (byte)8);
    NewNPC.Emit(OpCodes.Ldarg_S, (byte)9);
    NewNPC.Emit(OpCodes.Ldloc_1);         
    NewNPC.Emit(OpCodes.Call, callback);
#else
    NewNPC.Next.OpCode = OpCodes.Call;
    NewNPC.Next.Operand = callback;

    foreach (var prm in NewNPC.Method.Parameters)
        NewNPC.Emit(OpCodes.Ldarg, prm);
#endif

}

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class NPC
        {
            public class CreateEventArgs : EventArgs
            {
                public Terraria.NPC Npc { get; set; }
#if TerrariaServer_EntitySourcesActive || Terraria_EntitySourcesActive || tModLoader_EntitySourcesActive
                public IEntitySource Source { get; set; }
#endif

                public int X { get; set; }
                public int Y { get; set; }
                public int Type { get; set; }
                public int Start { get; set; }
                public float Ai0 { get; set; }
                public float Ai1 { get; set; }
                public float Ai2 { get; set; }
                public float Ai3 { get; set; }
                public int Target { get; set; }
#if Terraria_1457_OrAbove || TerrariaServer_1457_OrAbove
                public int Slot { get; set; }
                public int Generation { get; set; }
#endif
            }
            public static event EventHandler<CreateEventArgs> Create;

#if Terraria_1457_OrAbove || TerrariaServer_1457_OrAbove
            public static void InvokeCreate(int Slot, int Generation, IEntitySource source, int X, int Y, int Type, int Start, float ai0, float ai1, float ai2, float ai3, int Target, Terraria.NPC? npc = null)
#elif TerrariaServer_EntitySourcesActive || Terraria_EntitySourcesActive || tModLoader_EntitySourcesActive
            public static Terraria.NPC InvokeCreate(IEntitySource source, int X, int Y, int Type, int Start, float ai0, float ai1, float ai2, float ai3, int Target)
#else
            public static Terraria.NPC InvokeCreate(int X, int Y, int Type, int Start, float ai0, float ai1, float ai2, float ai3, int Target)
#endif
            {
                var args = new CreateEventArgs()
                {
#if TerrariaServer_EntitySourcesActive || Terraria_EntitySourcesActive || tModLoader_EntitySourcesActive
                    Source = source,
#endif
                    X = X,
                    Y = Y,
                    Type = Type,
                    Start = Start,
                    Ai0 = ai0,
                    Ai1 = ai1,
                    Ai2 = ai2,
                    Ai3 = ai3,
                    Target = Target
#if Terraria_1457_OrAbove || TerrariaServer_1457_OrAbove
                    , Slot = Slot
                    , Generation = Generation
                    , Npc = npc
#endif
                };
                Create?.Invoke(null, args);
#if Terraria_1457_OrAbove || TerrariaServer_1457_OrAbove

#else
                return args.Npc ?? new Terraria.NPC();
#endif
            }
        }
    }
}

