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
using MonoMod.Cil;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Rocks;

[Modification(ModType.PreMerge, "Removing NetMessage.SendData write buffer")]
void PatchSendDataWriter(MonoModder modder)
{
    var SendData = modder.GetILCursor(() => Terraria.NetMessage.SendData(default, default, default, default, default, default, default, default, default, default, default));

    // make it easier to track down variables
    SendData.Body.SimplifyMacros();

    var binaryWriter = SendData.Body.Variables.Single(v => v.VariableType.FullName == typeof(System.IO.BinaryWriter).FullName);

    var setters = SendData.Body.Instructions.Where(i => i.OpCode == OpCodes.Stloc && i.Operand == binaryWriter);

    if (setters.Count() != 2)
        throw new Exception($"{SendData.Method.FullName} expected 2 binaryWriter set instructions.");

    // the second set is only if this one yielded null; which it will never be after this
    var setter = setters.First();

    var stack = StackCounter.Count(SendData.Method);
    var stackOffset = stack.Single(s => s.Ins == setter);
    var initialVariable = stackOffset.FindPrevious(c => c.OnStackBefore == 0);

    // reapply
    SendData.Body.OptimizeMacros();
}