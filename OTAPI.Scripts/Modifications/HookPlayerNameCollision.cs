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
using Mono.Cecil.Cil;
using MonoMod;

[Modification(ModType.PostPatch, "Hooking player name collisions")]
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
    csr.EmitDelegate<PlayerNameCollisionCallback>(OTAPI.Callbacks.MessageBuffer.NameCollision);
    csr.Emit(OpCodes.Brfalse_S, flag.Operand as Instruction);
}

[MonoMod.MonoModIgnore]
public delegate bool PlayerNameCollisionCallback(Terraria.Player player);

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class MessageBuffer
        {
            public delegate HookResult NameCollisionHandler(Terraria.Player player);
            public static NameCollisionHandler NameCollision;
        }
    }
}

namespace OTAPI.Callbacks
{
    public static partial class MessageBuffer
    {
        public static bool NameCollision(Terraria.Player player)
        {
            return Hooks.MessageBuffer.NameCollision?.Invoke(player) != HookResult.Cancel;
        }
    }
}
