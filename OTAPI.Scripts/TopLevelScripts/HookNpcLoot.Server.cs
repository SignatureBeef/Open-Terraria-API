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

#if tModLoaderServer_V1_3
System.Console.WriteLine("Npc loot not available in TML1.3");
#else
using System;
using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;

/// <summary>
/// @doc Creates Hooks.NPC.DropLoot. Allows plugins to alter or cancel NPC loot drops.
/// </summary>
[Modification(ModType.PreMerge, "Hooking Terraria.GameContent.ItemDropRules.CommonCode.DropItemFromNPC")]
void HookNpcLoot(MonoModder modder)
{
    var NewNPC = modder.GetILCursor(() => Terraria.GameContent.ItemDropRules.CommonCode.DropItemFromNPC(default, default, default, default));

    NewNPC.GotoNext(
        i => i.OpCode == OpCodes.Call && i.Operand is MethodReference methodReference && methodReference.Name == "NewItem" && methodReference.DeclaringType.FullName == "Terraria.Item"
    );

    NewNPC.Emit(OpCodes.Ldarg_0); // NPC instance
#if TerrariaServer_EntitySourcesActive || Terraria_EntitySourcesActive
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

#if TerrariaServer_EntitySourcesActive || Terraria_EntitySourcesActive
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
                public bool NoGrabDelay { get; set; }
                public bool ReverseLookup { get; set; }
            }
            public static event EventHandler<DropLootEventArgs> DropLoot;

#if TerrariaServer_EntitySourcesActive || Terraria_EntitySourcesActive
            public static int InvokeDropLoot(Terraria.DataStructures.IEntitySource source, int X, int Y, int Width, int Height, int Type,
#else
            public static int InvokeDropLoot(int X, int Y, int Width, int Height, int Type,
#endif
                int Stack, bool noBroadcast, int pfix, bool noGrabDelay, bool reverseLookup,
                Terraria.NPC instance)
            {
                var args = new DropLootEventArgs()
                {
                    Event = HookEvent.Before,
#if TerrariaServer_EntitySourcesActive || Terraria_EntitySourcesActive
                    Source = source,
#endif
                    X = X,
                    Y = Y,
                    Width = Width,
                    Height = Height,
                    Type = Type,
                    Stack = Stack,
                    NoBroadcast = noBroadcast,
                    Pfix = pfix,
                    NoGrabDelay = noGrabDelay,
                    ReverseLookup = reverseLookup,
                    Npc = instance,

                    ItemIndex = 0,
                };
                DropLoot?.Invoke(null, args);
                if (args.Result != HookResult.Cancel)
                {
#if TerrariaServer_EntitySourcesActive || Terraria_EntitySourcesActive
                    args.ItemIndex = Terraria.Item.NewItem(args.Source, X, Y, Width, Height, Type, Stack, noBroadcast, pfix, noGrabDelay, reverseLookup);
#else
                    args.ItemIndex = Terraria.Item.NewItem(X, Y, Width, Height, Type, Stack, noBroadcast, pfix, noGrabDelay, reverseLookup);
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