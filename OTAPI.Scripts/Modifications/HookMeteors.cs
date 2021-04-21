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
using System.Linq;
using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;

[Modification(ModType.PreMerge, "Hooking meteors")]
void HookMeteors(ModFramework.ModFwModder modder)
{
    var csr = modder.GetILCursor(() => Terraria.WorldGen.meteor(0, 0, false));

    // look for stopDrops = true in the vanilla methods instructions
    csr.GotoNext(MonoMod.Cil.MoveType.Before, instruction =>
         instruction.OpCode == OpCodes.Stsfld
        && instruction.Operand is FieldReference
        && (instruction.Operand as FieldReference).Name == "stopDrops"

        && instruction.Previous.OpCode == OpCodes.Ldc_I4_1);

    var insContinue = csr.Next;

    csr.EmitAll(
        //Create the callback execution
        new { OpCodes.Ldarga, Operand = csr.Method.Parameters[0] }, //reference to int x
        new { OpCodes.Ldarga, Operand = csr.Method.Parameters[1] } //reference to int y
    );

    csr.EmitDelegate<MeteorCallback>(OTAPI.Callbacks.WorldGen.Meteor);

    csr.EmitAll(
        //If the callback is not canceled, continue on with vanilla code
        new { OpCodes.Brtrue_S, insContinue },

        //If the callback is canceled, return false
        new { OpCodes.Ldc_I4_0 }, //false
        new { OpCodes.Ret } //return
    );
}

[MonoMod.MonoModIgnore]
public delegate bool MeteorCallback(ref int x, ref int y);

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class WorldGen
        {
            public delegate HookResult MeteorHandler(ref int x, ref int y);
            public static MeteorHandler Meteor;
        }
    }
}

namespace OTAPI.Callbacks
{
    public static partial class WorldGen
    {
        public static bool Meteor(ref int x, ref int y)
        {
            return OTAPI.Hooks.WorldGen.Meteor?.Invoke(ref x, ref y) != HookResult.Cancel;
        }
    }
}