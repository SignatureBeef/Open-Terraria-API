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
using Mono.Cecil.Cil;
using MonoMod;

/// <summary>
/// @doc Creates Hooks.MessageBuffer.NameCollision. Allows plugins to control and cancel name collisions when 2 players join with the same name.
/// </summary>
[Modification(ModType.PrePatch, "Hooking player name collisions")]
[MonoMod.MonoModIgnore]
void HookPlayerNameCollision(MonoModder modder)
{
    int tmp;
    var csr = modder.GetILCursor(() => (new Terraria.MessageBuffer()).GetData(0, 0, out tmp));

    var flag = csr.Body.Instructions
        .Single(x => x.OpCode == OpCodes.Ldstr && x.Operand.Equals("Net.NameTooLong"))
        .Previous(y => y.OpCode == OpCodes.Brfalse_S);

    var player = flag.Next(x => x.OpCode == OpCodes.Ldloc_S);

    csr.Goto(flag, MonoMod.Cil.MoveType.After);

    csr.Emit(OpCodes.Ldloc_S, player.Operand as VariableDefinition);
    csr.EmitDelegate(OTAPI.Hooks.MessageBuffer.InvokeNameCollision);
    csr.Emit(OpCodes.Brfalse_S, flag.Operand as Instruction);
}

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class MessageBuffer
        {
            public class NameCollisionEventArgs : EventArgs
            {
                public HookEvent Event { get; set; }
                public HookResult? Result { get; set; }

                public Terraria.Player Player { get; set; }
            }
            public static event EventHandler<NameCollisionEventArgs>? NameCollision;

            public static bool InvokeNameCollision(Terraria.Player player)
            {
                var args = new NameCollisionEventArgs()
                {
                    Player = player
                };
                NameCollision?.Invoke(null, args);
                return args.Result != HookResult.Cancel;
            }
        }
    }
}
