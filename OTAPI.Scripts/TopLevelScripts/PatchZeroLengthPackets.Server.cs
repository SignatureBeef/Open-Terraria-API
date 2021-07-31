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

using ModFramework;
using Mono.Cecil.Cil;
using MonoMod;
using MonoMod.Cil;

/// <summary>
/// @doc Prevent infinite server loop. refer to https://github.com/Pryaxis/TShock/issues/1673
/// </summary>
[Modification(ModType.PreMerge, "Patching zero-length packet exploits")]
void PatchZeroLengthPackets(MonoModder modder)
{
    // find the only instance of ToUint16, get it's stored variable and add a 0 length check to break out
    var checkBytes = modder.GetILCursor(() => Terraria.NetMessage.CheckBytes(0));

    var toUInt16 = checkBytes.GotoNext(MoveType.After, (ins) => ins.MatchCall("System.BitConverter", "ToUInt16"));
    var storageVariable = toUInt16.Next.Operand as Mono.Cecil.Cil.VariableReference;
    var breakTo = toUInt16.GotoNext((ins) => ins.OpCode == OpCodes.Blt || ins.OpCode == OpCodes.Blt_S).Next.Operand as Instruction;

    toUInt16.Index++;
    toUInt16
        .Emit(OpCodes.Ldloc_S, storageVariable)
        .Emit(OpCodes.Ldc_I4_0)
        .Emit(OpCodes.Beq, breakTo);
}