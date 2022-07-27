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

using System;
using System.Linq;
using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;

/// <summary>
/// @doc Creates Hooks.Wiring.AnnouncementBox. Allows plu
/// </summary>
[Modification(ModType.PreMerge, "Hooking wiring announce box")]
[MonoMod.MonoModIgnore]
void HookWiringAnnounceBox(MonoModder modder)
{
    var csr = modder.GetILCursor(() => Terraria.Wiring.HitWireSingle(0, 0));

    if (csr.Method.Parameters.Count != 2)
        throw new NotSupportedException("Expected 2 parameters for the callback");

    var insertionPoint = csr.Body.Instructions.First(
        x => x.OpCode == OpCodes.Ldsfld
        && (x.Operand as FieldReference).Name == "AnnouncementBoxRange"
    );

    var signVariable = csr.Body.Instructions.First(
        x => x.OpCode == OpCodes.Call
        && (x.Operand as MethodReference).Name == "ReadSign"
    ).Next.Operand;

    csr.Goto(insertionPoint, MonoMod.Cil.MoveType.Before);

    var injectedInstructions = csr.EmitAll(
        new { OpCodes.Ldarg_0 },
        new { OpCodes.Ldarg_1 },
        new { OpCodes.Ldloc_S, Operand = signVariable as VariableDefinition }
    );

    csr.EmitDelegate(OTAPI.Hooks.Wiring.InvokeAnnouncementBox);

    insertionPoint.ReplaceTransfer(injectedInstructions.First(), csr.Method);

    csr.EmitAll(
        new { OpCodes.Brtrue_S, insertionPoint },
        new { OpCodes.Ret }
    );
}

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Wiring
        {
            public class AnnouncementBoxEventArgs : EventArgs
            {
                public HookResult? Result { get; set; }

                public int X { get; set; }
                public int Y { get; set; }
                public int SignId { get; set; }
            }
            public static event EventHandler<AnnouncementBoxEventArgs> AnnouncementBox;

            public static bool InvokeAnnouncementBox(int x, int y, int signId)
            {
                var args = new Hooks.Wiring.AnnouncementBoxEventArgs()
                {
                    X = x,
                    Y = y,
                    SignId = signId,
                };
                AnnouncementBox?.Invoke(null, args);
                return args.Result != HookResult.Cancel;
            }
        }
    }
}
