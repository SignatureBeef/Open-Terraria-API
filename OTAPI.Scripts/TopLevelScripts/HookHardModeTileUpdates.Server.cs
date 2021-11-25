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
using System.Linq;

/// <summary>
/// @doc Creates Hooks.WorldGen.HardmodeTileUpdate. Allows plugins to intercept hard mode tile updates.
/// </summary>
[Modification(ModType.PreMerge, "Hooking hardmode tile updates")]
[MonoMod.MonoModIgnore]
void HardModeTileUpdates(MonoModder modder)
{
    var tile = modder.GetFieldDefinition(() => Terraria.Main.tile);
    var tileType = (tile.FieldType as ArrayType).ElementType;
    var csr = modder.GetILCursor(() => Terraria.WorldGen.hardUpdateWorld(0, 0));

    var targets = csr.Body.Instructions.Where(ins =>
        //(instruction.Operand is FieldReference fieldRef) && fieldRef.FullName == tile.FullName
        ins.OpCode == OpCodes.Stfld
        && ins.Operand is FieldReference fieldRef
        && fieldRef.DeclaringType.FullName == tileType.FullName
        && fieldRef.Name == "type"
    ).ToArray();

    foreach (var match in targets)
    {
        csr.Goto(match);

        // move back to the tile collection being pushed on the stack, we will wrap this up until SendTileSquare
        csr.GotoPrev(MoveType.Before, ins => ins.OpCode == OpCodes.Ldsfld && (ins.Operand as FieldReference).Name == "tile");

        // store this for later. otherwise we need to cycle back to here, so its easier to just store it upfront
        var startOfBranch = csr.Next;

        // find the instruction to where our cancel code should end up.
        csr.FindNext(out ILCursor[] continuation, ins => ins.OpCode == OpCodes.Call && (ins.Operand as MethodReference).Name == "SendTileSquare");

        if (continuation.Length != 1)
            throw new Exception($"{nameof(Terraria.WorldGen.hardUpdateWorld)} unable to determine continuation branch.");

        var continueBranch = continuation[0].Next.Next;

        // nop as a placeholder. it must take the transfer from other branches otherwise the if conditions won't align correctly.
        var nop = csr.Emit(OpCodes.Nop).Prev;
        startOfBranch.ReplaceTransfer(nop, csr.Method);

        // add the x/y (params)
        var param = startOfBranch.Next;
        while (param.OpCode.FlowControl != FlowControl.Call)
        {
            csr.Emit(param.OpCode, param.Operand);
            param = param.Next;
        }

        // find the tile type instruction and add it to our param stack.
        csr.FindNext(out ILCursor[] csrTypes, ins => ins.OpCode == OpCodes.Ldc_I4_S || ins.OpCode == OpCodes.Ldc_I4);
        if (csrTypes.Length != 1)
            throw new Exception($"{nameof(Terraria.WorldGen.hardUpdateWorld)} unable to determine type instructions.");

        csr.Emit(csrTypes[0].Next.OpCode, csrTypes[0].Next.Operand);

        csr.EmitDelegate<HardmodeTilePlaceCallback>(OTAPI.Hooks.WorldGen.InvokeHardmodeTileUpdate);
        csr.Emit(OpCodes.Brfalse_S, continueBranch);
    }
}

[MonoMod.MonoModIgnore]
public delegate bool HardmodeTilePlaceCallback(int x, int y, ushort type);

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class WorldGen
        {
            public class HardmodeTileUpdateEventArgs : EventArgs
            {
                public HookResult? Result { get; set; }

                public int X { get; set; }
                public int Y { get; set; }
                public int Type { get; set; }
            }
            public static event EventHandler<HardmodeTileUpdateEventArgs> HardmodeTileUpdate;

            public static bool InvokeHardmodeTileUpdate(int x, int y, ushort type)
            {
                var args = new HardmodeTileUpdateEventArgs()
                {
                    X = x,
                    Y = y,
                    Type = type,
                };

                HardmodeTileUpdate?.Invoke(null, args);

                if (args.Result == HookResult.Cancel)
                    return false;

                return true;
            }
        }
    }
}