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

#if tModLoaderServer_V1_3 || tModLoader_V1_4
System.Console.WriteLine("Npc loot not available in TML");
#else
using System;
using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;
using Terraria;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

/// <summary>
/// @doc Creates Hooks.NPC.DropLoot. Allows plugins to alter or cancel NPC loot drops.
/// </summary>
[Modification(ModType.PreMerge, "Hooking Terraria.GameContent.ItemDropRules.CommonCode.DropItemFromNPC")]
[MonoMod.MonoModIgnore]
void HookNpcLoot(MonoModder modder)
{
    var NewNPC = modder.GetILCursor(() => Terraria.GameContent.ItemDropRules.CommonCode.DropItemFromNPC(default, default, default, default));

    NewNPC.GotoNext(
        i => i.OpCode == OpCodes.Call && i.Operand is MethodReference methodReference && methodReference.Name == "NewItem" && methodReference.DeclaringType.FullName == "Terraria.Item"
    );

    NewNPC.Emit(OpCodes.Ldarg_0); // NPC instance
#if Terraria_1457_OrAbove || TerrariaServer_1457_OrAbove
    NewNPC.Next.Operand = modder.GetMethodDefinition(() => OTAPI.Hooks.NPC.InvokeDropLoot(null, 0, 0, 0, 0, 0, 0, false, 0, NewItemOwnership.None, null, null, null));
#elif TerrariaServer_1450_OrAbove || Terraria_1450_OrAbove || tModLoader_1450_OrAbove
    NewNPC.Next.Operand = modder.GetMethodDefinition(() => OTAPI.Hooks.NPC.InvokeDropLoot(default, default, default, default, default, default, default, default, default, default, default));
#elif TerrariaServer_EntitySourcesActive || Terraria_EntitySourcesActive || tModLoader_EntitySourcesActive
    NewNPC.Next.Operand = modder.GetMethodDefinition(() => OTAPI.Hooks.NPC.InvokeDropLoot(default, default, default, default, default, default, default, default, default, default, default, default));
#else
    NewNPC.Next.Operand = modder.GetMethodDefinition(() => OTAPI.Hooks.NPC.InvokeDropLoot(default, default, default, default, default, default, default, default, default, default, default));
#endif
}

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class NPC
        {
            public class DropLootEventArgs : EventArgs
            {
                public HookEvent Event { get; set; }
                public HookResult? Result { get; set; }

#if TerrariaServer_EntitySourcesActive || Terraria_EntitySourcesActive || tModLoader_EntitySourcesActive
                public Terraria.DataStructures.IEntitySource Source { get; set; }
#endif
                public Terraria.NPC Npc { get; set; }
                public int ItemIndex { get; set; }
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
            public static event EventHandler<DropLootEventArgs>? DropLoot;

#if TerrariaServer_EntitySourcesActive || Terraria_EntitySourcesActive || tModLoader_EntitySourcesActive
            public static int InvokeDropLoot(Terraria.DataStructures.IEntitySource source, int X, int Y, int Width, int Height, int Type,
#else
            public static int InvokeDropLoot(int X, int Y, int Width, int Height, int Type,
#endif

#if Terraria_1457_OrAbove || TerrariaServer_1457_OrAbove
                int stack, bool noBroadcast, int prefix, NewItemOwnership ownership, Vector2? velocity, Item.NewItemModifier modifier,
#elif TerrariaServer_1450_OrAbove || Terraria_1450_OrAbove || tModLoader_1450_OrAbove
                int Stack, bool noBroadcast, int pfix, bool noGrabDelay,
#else            
                int Stack, bool noBroadcast, int pfix, bool noGrabDelay, bool reverseLookup,
#endif
                Terraria.NPC instance)
            {
                var args = new DropLootEventArgs()
                {
                    Event = HookEvent.Before,
#if TerrariaServer_EntitySourcesActive || Terraria_EntitySourcesActive || tModLoader_EntitySourcesActive
                    Source = source,
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
                    Npc = instance,

                    ItemIndex = 0,
                };
                DropLoot?.Invoke(null, args);
                if (args.Result != HookResult.Cancel)
                {
#if Terraria_1457_OrAbove || TerrariaServer_1457_OrAbove
                    args.ItemIndex = Terraria.Item.NewItem(args.Source, args.X, args.Y, args.Width, args.Height, args.Type, args.Stack, args.NoBroadcast, args.Pfix, args.Ownership, args.Velocity, args.Modifier);
#elif TerrariaServer_1450_OrAbove || Terraria_1450_OrAbove || tModLoader_1450_OrAbove
                    args.ItemIndex = Terraria.Item.NewItem(args.Source, args.X, args.Y, args.Width, args.Height, args.Type, args.Stack, args.NoBroadcast, args.Pfix, args.NoGrabDelay);
#elif TerrariaServer_EntitySourcesActive || Terraria_EntitySourcesActive || tModLoader_EntitySourcesActive
                    args.ItemIndex = Terraria.Item.NewItem(args.Source, args.X, args.Y, args.Width, args.Height, args.Type, args.Stack, args.NoBroadcast, args.Pfix, args.NoGrabDelay, args.ReverseLookup);
#else
                    args.ItemIndex = Terraria.Item.NewItem(args.X, args.Y, args.Width, args.Height, args.Type, args.Stack, args.NoBroadcast, args.Pfix, args.NoGrabDelay, args.ReverseLookup);
#endif
                    args.Event = HookEvent.After;
                    DropLoot?.Invoke(null, args);
                }
                return args.ItemIndex;
            }
        }
    }
}

#endif