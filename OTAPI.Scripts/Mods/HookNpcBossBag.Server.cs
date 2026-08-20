/*
Copyright (C) 2020-2024 SignatureBeef & sors89

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
using System.Linq;
using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Terraria;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

/// <summary>
/// @doc Creates Hooks.NPC.BossBag. Allows plugins to customize boss loot as well as the distribution.
/// </summary>
[Modification(ModType.PreMerge, "Hooking NPC Boss Bag drops")]
[MonoMod.MonoModIgnore]
void HookNpcBossBag(ModFramework.ModFwModder modder)
{
    // replace NewItem calls, and handle the -1 result to cancel the method from actioning.
    foreach (var csr in new[] {
#if !Terraria_1457_OrAbove && !TerrariaServer_1457_OrAbove
         modder.GetILCursor(() => (new Terraria.NPC()).DropItemInstanced(default, default, 0, 0, false)),
#endif
         modder.GetILCursor(() => Terraria.GameContent.ItemDropRules.CommonCode.DropItemLocalPerClientAndSetNPCMoneyTo0(default, default, default, default))
    })
    {
        var callback = csr.Module.ImportReference(
#if Terraria_1457_OrAbove || TerrariaServer_1457_OrAbove
            modder.GetMethodDefinition(() => OTAPI.Hooks.NPC.InvokeBossBag(null, 0, 0, 0, 0, 0, 0, false, 0, NewItemOwnership.None, null, null, null))
#elif TerrariaServer_1450_OrAbove || Terraria_1450_OrAbove || tModLoader_1450_OrAbove
            modder.GetMethodDefinition(() => OTAPI.Hooks.NPC.InvokeBossBag(null, 0, 0, 0, 0, 0, 0, false, 0, false, null))
#elif TerrariaServer_EntitySourcesActive || Terraria_EntitySourcesActive || tModLoader_EntitySourcesActive
            modder.GetMethodDefinition(() => OTAPI.Hooks.NPC.InvokeBossBag(null, 0, 0, 0, 0, 0, 0, false, 0, false, false, null))
#else
            modder.GetMethodDefinition(() => OTAPI.Hooks.NPC.InvokeBossBag(0, 0, 0, 0, 0, 0, false, 0, false, false, null))
#endif
        );

        var instructions = csr.Body.Instructions.Where(x => x.OpCode == OpCodes.Call
                                                            && x.Operand is MethodReference mref && mref.Name == "NewItem"
#if Terraria_1457_OrAbove || TerrariaServer_1457_OrAbove
                                                            && x.Next.OpCode == OpCodes.Stloc_1);
#else
                                                            && x.Next.OpCode == OpCodes.Stloc_0);
#endif

        if (instructions.Count() != 1) throw new NotSupportedException("Only one server NewItem call expected in DropBossBags.");

        var ins = instructions.First();

        ins.Operand = callback;

        csr.Goto(ins);
        csr.EmitAll(
             // reference to the npc works for both the instance, and the static method, only since the latter has the 
             // variable first in the argument list.
            new { OpCodes.Ldarg_0 }
        );

        csr.Goto(ins.Next.Next);
#if Terraria_1457_OrAbove || TerrariaServer_1457_OrAbove
        csr.EmitAll(
            new { OpCodes.Ldloc_1 },
            new { OpCodes.Ldc_I4_M1 },
            new { OpCodes.Ceq },
            new { OpCodes.Brfalse_S, Operand = ins.Next.Next },
            new { OpCodes.Ret }
        );
#else
        csr.EmitAll(
            new { OpCodes.Ldloc_0 },
            new { OpCodes.Ldc_I4_M1 },
            new { OpCodes.Ceq },
            new { OpCodes.Brfalse_S, Operand = ins.Next.Next },
            new { OpCodes.Ret }
        );
#endif
    }
}

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class NPC
        {
            public class BossBagEventArgs : EventArgs
            {
                public HookResult? Result { get; set; }

#if TerrariaServer_EntitySourcesActive || Terraria_EntitySourcesActive || tModLoader_EntitySourcesActive
                public IEntitySource Source { get; set; }
#endif

                public Terraria.NPC Npc { get; set; }
                public int X { get; set; }
                public int Y { get; set; }
                public int Width { get; set; }
                public int Height { get; set; }
                public int Type { get; set; }
                public int Stack { get; set; }
                public bool NoBroadcast { get; set; }
                public int Pfix { get; set; }
#if Terraria_1457_OrAbove || TerrariaServer_1457_OrAbove
                [Obsolete("NoGrabDelay is no longer used in Terraria 1.4.5.7 and above, but is kept for API compatibility.")]
#endif
                public bool NoGrabDelay { get; set; }
#if TerrariaServer_1450_OrAbove || Terraria_1450_OrAbove || tModLoader_1450_OrAbove
                [Obsolete("ReverseLookup is no longer used in Terraria 1.4.5 and above, but is kept for API compatibility.")]
#endif
                public bool ReverseLookup { get; set; }
#if Terraria_1457_OrAbove || TerrariaServer_1457_OrAbove
                public NewItemOwnership Ownership { get; set; }
                public Vector2? Velocity { get; set; }
                public Item.NewItemModifier Modifier { get; set; }
#endif
            }
            public static event EventHandler<BossBagEventArgs> BossBag;

            public static int InvokeBossBag(
#if TerrariaServer_EntitySourcesActive || Terraria_EntitySourcesActive || tModLoader_EntitySourcesActive
                IEntitySource Source,
#endif
                int X,
                int Y,
                int Width,
                int Height,
                int Type,
//                 int Stack,
//                 bool noBroadcast,
//                 int pfix,
//                 bool noGrabDelay,
// #if !TerrariaServer_1450_OrAbove && !Terraria_1450_OrAbove && !tModLoader_1450_OrAbove
//                 bool reverseLookup,
// #endif

#if Terraria_1457_OrAbove || TerrariaServer_1457_OrAbove
                int stack, bool noBroadcast, int prefix, NewItemOwnership ownership, Vector2? velocity, Item.NewItemModifier modifier,
#elif TerrariaServer_1450_OrAbove || Terraria_1450_OrAbove || tModLoader_1450_OrAbove
                int Stack, bool noBroadcast, int pfix, bool noGrabDelay,
#else            
                int Stack, bool noBroadcast, int pfix, bool noGrabDelay, bool reverseLookup,
#endif
                Terraria.NPC npc
            )
            {
                var args = new BossBagEventArgs()
                {
#if TerrariaServer_EntitySourcesActive || Terraria_EntitySourcesActive || tModLoader_EntitySourcesActive
                    Source = Source,
#endif
                    X = X,
                    Y = Y,
                    Width = Width,
                    Height = Height,
                    Type = Type,
                    NoBroadcast = noBroadcast,
#if Terraria_1457_OrAbove || TerrariaServer_1457_OrAbove
                    Stack = stack,
                    Pfix = prefix,
                    Ownership = ownership,
                    Velocity = velocity,
                    Modifier = modifier,
                    NoGrabDelay = false, // no longer used, but kept for api compatibility.
#else
                    Stack = Stack,
                    Pfix = pfix,
                    NoGrabDelay = noGrabDelay,
#endif
#if TerrariaServer_1450_OrAbove || Terraria_1450_OrAbove || tModLoader_1450_OrAbove
                    ReverseLookup = false, // no longer used, but kept for api compatibility.
#else
                    ReverseLookup = reverseLookup,
#endif
                    Npc = npc,
                };
                BossBag?.Invoke(null, args);
                if (args.Result == HookResult.Cancel)
                    return -1;

#if Terraria_1457_OrAbove || TerrariaServer_1457_OrAbove
                return Terraria.Item.NewItem(Source, args.X, args.Y, args.Width, args.Height, args.Type, args.Stack, args.NoBroadcast, args.Pfix, args.Ownership, args.Velocity, args.Modifier);
#elif TerrariaServer_1450_OrAbove || Terraria_1450_OrAbove || tModLoader_1450_OrAbove
                return Terraria.Item.NewItem(Source, args.X, args.Y, args.Width, args.Height, args.Type, args.Stack, args.NoBroadcast, args.Pfix, args.NoGrabDelay);
#elif TerrariaServer_EntitySourcesActive || Terraria_EntitySourcesActive || tModLoader_EntitySourcesActive
                return Terraria.Item.NewItem(Source, args.X, args.Y, args.Width, args.Height, args.Type, args.Stack, args.NoBroadcast, args.Pfix, args.NoGrabDelay, args.ReverseLookup);
#else
                return Terraria.Item.NewItem(args.X, args.Y, args.Width, args.Height, args.Type, args.Stack, args.NoBroadcast, args.Pfix, args.NoGrabDelay, args.ReverseLookup);
#endif
            }
        }
    }
}
