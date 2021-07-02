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

/// <summary>
/// @doc Removes sync lock functionality in Terraria.NetMessage.SendData, used in conjunction with the SendDataWriter patch.
/// </summary>
[Modification(ModType.PreMerge, "Removing NetMessage.SendData locks")]
void PatchSendDataLocks(MonoModder modder)
{
    var SendData = modder.GetILCursor(() => Terraria.NetMessage.SendData(0, 0, 0, null, 0, 0, 0, 0, 0, 0, 0));

    var locks = FindLocks(SendData);

    if (locks.Count() != 1)
        throw new Exception($"{SendData.Method.FullName} expected only 1 lock.");

    var the_lock = locks.Single();

    // transform short codes to actual variable references.
    // this saves a bit of complexity in finding the variables to be removed
    SendData.Body.SimplifyMacros();

    var startOfLock = FindStartOfLock(SendData, the_lock);

    // leave any branches/transfers intact
    startOfLock.Ins.OpCode = OpCodes.Nop;
    startOfLock.Ins.Operand = null;
    startOfLock = startOfLock.Next;

    // remove the start of the lock
    SendData.Goto(startOfLock.Ins);
    SendData.RemoveWhile(ins => !(ins.Operand is MethodReference mref
            && mref.DeclaringType.FullName == "System.Threading.Monitor"
            && mref.Name == "Enter"));

    // remove the handler
    SendData.Goto(the_lock.HandlerStart);
    SendData.RemoveWhile(ins => SendData.Next.OpCode != OpCodes.Endfinally);

    // remove the now useless try/finally
    SendData.Body.ExceptionHandlers.Remove(the_lock);

    // reapply optimisations that we undone
    SendData.Body.OptimizeMacros();
}

ILCount FindStartOfLock(ILCursor cursor, ExceptionHandler exlock)
{
    cursor.Goto(exlock.TryStart);

    if (exlock.TryStart.OpCode != OpCodes.Ldloc)
        throw new Exception($"{cursor.Method.FullName} unable to determine the reference variable to remove the lock.");

    var variable = (VariableDefinition)exlock.TryStart.Operand;

    cursor.GotoPrev(ins => ins.OpCode == OpCodes.Stloc && ins.Operand == variable);

    var count = cursor.Method.GetStack();
    var offset = count.Single(x => x.Ins == cursor.Next);

    return offset.FindRoot();
}

IEnumerable<ExceptionHandler> FindLocks(ILCursor cursor)
{
    // look for all endfinally instructions, and if the previous instruction is a Monitor.Exit we can assume it's a lock

    var endfinally = cursor.Body.Instructions.Where(i => i.OpCode == OpCodes.Endfinally
        && i.Previous?.Operand is MethodReference mref
        && mref.DeclaringType.FullName == "System.Threading.Monitor"
        && mref.Name == "Exit"
    );

    return cursor.Body.ExceptionHandlers.Where(x => x.HandlerType == ExceptionHandlerType.Finally &&
        endfinally.Any(ef => ef.Next == x.HandlerEnd));
}

