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
#pragma warning disable CS0436 // Type conflicts with imported type

using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

/// <summary>
/// @doc Creates Hooks.Chest.QuickStack.
/// </summary>
[MonoModIgnore]
partial class ChestHooks
{
    static MethodDefinition PutItemInNearbyChest { get; set; }
    static ParameterDefinition PlayerID { get; set; }

    [Modification(ModType.PreMerge, "Hooking chest stacking")]
    static void HookChestQuickStack(ModFwModder modder)
    {
#if TerrariaServer_1450_OrAbove || Terraria_1450_OrAbove || tModLoader_1450_OrAbove
        {
            // DO NOT use GetILCursor(). The instruction body does not have jumps transformed into labels,
            // a process which is required to happen for the edits below to work.
            var ctx = new ILContext(modder.GetMethodDefinition(() => QuickStacking.BuildDestinationMetricsAndStackItems(default, default, default)));

            // Hooks onto quick stacking into existing slot
            ctx.Invoke((ctx) =>
            {
                var csr = new ILCursor(ctx);

                ILLabel endLabel = null!;
                csr.GotoNext(
                    MoveType.After,
                    i => i.MatchLdarg(1),
                    i => i.MatchLdcI4(1),
                    i => i.MatchStfld(typeof(QuickStacking.DestinationHelper), nameof(QuickStacking.DestinationHelper.transferBlocked)),
                    i => i.MatchBr(out endLabel)
                );
                // AfterLabel is not the same as After + MoveAfterLabels for some reason
                csr.MoveAfterLabels();

                // Load player ID (source.slots[0].Player.whoAmI)
                // source
                csr.Emit(OpCodes.Ldarg_0);
                // ^.slots
                csr.Emit(OpCodes.Ldfld, modder.GetFieldDefinition(() => default(QuickStacking.SourceInventory).slots));
                // ^[0]
                csr.Emit(OpCodes.Ldc_I4_0);
                csr.Emit(OpCodes.Ldelem_Any, modder.GetDefinition<PlayerItemSlotID.SlotReference>());
                // ^.Player
                csr.Emit(OpCodes.Ldfld, modder.GetFieldDefinition(() => default(PlayerItemSlotID.SlotReference).Player));
                // ^.whoAmI
                csr.Emit(OpCodes.Ldfld, modder.GetFieldDefinition(() => default(Player)!.whoAmI));

                // Load item
                // NOTE: this is very fragile, but I can't think of a way to write it better without significantly bloating the code
                csr.Emit(OpCodes.Ldloc_S, (byte)3);

                // Load chest index
                csr.Emit(OpCodes.Ldarg_S, (byte)1);
                // ^.ChestIndex (property get)
                csr.Emit(OpCodes.Callvirt, modder.GetDefinition<QuickStacking.DestinationHelper>().Properties.Single(p => p.Name == "ChestIndex")!.GetMethod);

                // Call hook
                csr.Emit(OpCodes.Call, modder.GetMethodDefinition(() => OTAPI.Hooks.Chest.InvokeQuickStack(default, default!, default)));

                // Continue if handled
                csr.Emit(OpCodes.Brfalse, endLabel);
            });
        }
        {
            // used for the out parameter only
            List<int> _blockedChests;

            // DO NOT use GetILCursor(). The instruction body does not have jumps transformed into labels,
            // a process which is required to happen for the edits below to work.
            var ctx = new ILContext(modder.GetMethodDefinition(() => QuickStacking.Transfer(default, default, out _blockedChests, default)));

            // Hooks onto quick stacking overflowing into a new stack
            ctx.Invoke((ctx) =>
            {
                var csr = new ILCursor(ctx);
                int count = 0;

                while (csr.TryGotoNext(
                    MoveType.Before,
                    i => i.MatchLdarg(0),
                    i => i.MatchLdloc(out _),
                    i => i.MatchCall(typeof(QuickStacking), nameof(QuickStacking.Consolidate))
                ))
                {
                    if (++count > 2)
                    {
                        throw new Exception($"More than two matches of {nameof(QuickStacking.Consolidate)}.");
                    }

                    // Both targets are preceded by a jump instruction exiting the loop
                    var endInstruction = csr.Prev.Operand;

                    csr.MoveAfterLabels();

                    // Load player ID (source.slots[0].Player.whoAmI)
                    // source
                    csr.Emit(OpCodes.Ldarg_0);
                    // ^.slots
                    csr.Emit(OpCodes.Ldfld, modder.GetFieldDefinition(() => default(QuickStacking.SourceInventory).slots));
                    // ^[0]
                    csr.Emit(OpCodes.Ldc_I4_0);
                    csr.Emit(OpCodes.Ldelem_Any, modder.GetDefinition<PlayerItemSlotID.SlotReference>());
                    // ^.Player
                    csr.Emit(OpCodes.Ldfld, modder.GetFieldDefinition(() => default(PlayerItemSlotID.SlotReference).Player));
                    // ^.whoAmI
                    csr.Emit(OpCodes.Ldfld, modder.GetFieldDefinition(() => default(Player)!.whoAmI));

                    // Load item
                    // NOTE: this is very fragile, but I can't think of a way to write it better without significantly bloating the code
                    csr.Emit(OpCodes.Ldloc_S, (byte)(count == 1 ? 6 : 9));

                    // Load chest index
                    // NOTE: this is very fragile, but I can't think of a way to write it better without significantly bloating the code
                    csr.Emit(OpCodes.Ldloc_S, (byte)(count == 1 ? 7 : 10));
                    // ^.ChestIndex (property get)
                    csr.Emit(OpCodes.Callvirt, modder.GetDefinition<QuickStacking.DestinationHelper>().Properties.Single(p => p.Name == "ChestIndex")!.GetMethod);

                    // Call hook
                    csr.Emit(OpCodes.Call, modder.GetMethodDefinition(() => OTAPI.Hooks.Chest.InvokeQuickStack(default, default!, default)));

                    // Continue if handled
                    csr.Emit(OpCodes.Brfalse, endInstruction);

                    // Move after Consolidate call
                    csr.Goto(csr.Index + 3);
                }
            });
        }
#else
        var csr = modder.GetILCursor(() => Terraria.Chest.PutItemInNearbyChest(null, default));
        PutItemInNearbyChest = csr.Method;

        modder.OnRewritingMethodBody += Modder_OnRewritingMethodBody;

        // add playerID for the hook to consume, then we need to rewrite all the uses of it to add it in.
        PutItemInNearbyChest.Parameters.Add(PlayerID = new ParameterDefinition(
            "playerID",
             ParameterAttributes.None,
            modder.Module.TypeSystem.Int32
        ));

        // inject the hook
        {
#if TerrariaServer_1441_OrAbove
            var beginInstruction = csr.Method.Body.Instructions.Single(x => x.OpCode == OpCodes.Bge);
#else
            var beginInstruction = csr.Method.Body.Instructions.Single(x => x.OpCode == OpCodes.Bge_Un);
#endif
            var endInstruction = beginInstruction.Next(x => x.OpCode == OpCodes.Ldc_I4_0);

            csr.Goto(beginInstruction, MoveType.After);

            csr.EmitAll(
                new { OpCodes.Ldarg, Operand = csr.Method.Parameters.Skip(2).SingleOrDefault() },
                new { OpCodes.Ldarg, Operand = csr.Method.Parameters.First() },
#if TerrariaServer_1441_OrAbove
                new { OpCodes.Ldloc_1 },
#else
                new { OpCodes.Ldloc_0 },
#endif
            new { OpCodes.Call, Operand = modder.GetMethodDefinition(() => OTAPI.Hooks.Chest.InvokeQuickStack(0, null, 0)) },
                new { OpCodes.Brtrue, endInstruction },
                new { OpCodes.Br, Operand = (Instruction)beginInstruction.Operand }
            );
        }
#endif
    }

    private static void Modder_OnRewritingMethodBody(MonoModder modder, MethodBody body, Instruction instr, int instri)
    {
        if (instr.Operand is MethodReference methodReference)
        {
            if (methodReference.DeclaringType.Name == PutItemInNearbyChest.DeclaringType.Name
                && methodReference.Name == PutItemInNearbyChest.Name)
            {
                if (methodReference.Parameters.Any(x => x.Name == PlayerID.Name))
                {
                    return;
                }
                methodReference.Parameters.Add(PlayerID);

                switch (body.Method.Name)
                {
                    case "ServerPlaceItem":
                        // add the player id from the first argument
                        body.GetILProcessor().InsertBefore(instr, new
                        {
                            OpCodes.Ldarg_0
                        });

                        break;
                    case "QuickStackAllChests":
                        body.GetILProcessor().InsertBefore(instr,
                            new { OpCodes.Ldarg_0, },
#if !tModLoaderServer_V1_3
                            new { OpCodes.Ldfld, Operand = (FieldReference)modder.GetFieldDefinition(() => (new Terraria.Player()).whoAmI) }
#else
                            new { OpCodes.Ldfld, Operand = (FieldReference)modder.GetFieldDefinition(() => (new Terraria.Player(true)).whoAmI) }
#endif
                        );
                        break;
                    default:
                        throw new NotImplementedException($"{body.Method.Name} is not a supported caller for this modification");
                }
            }
        }
    }
}

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Chest
        {
            public class QuickStackEventArgs : EventArgs
            {
                public HookResult? Result { get; set; }

                public int PlayerId { get; set; }
                public Terraria.Item Item { get; set; }
                public int ChestIndex { get; set; }
            }
            public static event EventHandler<QuickStackEventArgs> QuickStack;

            public static bool InvokeQuickStack(int playerId, Terraria.Item item, int chestIndex)
            {
                var args = new QuickStackEventArgs()
                {
                    PlayerId = playerId,
                    Item = item,
                    ChestIndex = chestIndex,
                };
                QuickStack?.Invoke(null, args);
                return args.Result != HookResult.Cancel;
            }
        }
    }
}