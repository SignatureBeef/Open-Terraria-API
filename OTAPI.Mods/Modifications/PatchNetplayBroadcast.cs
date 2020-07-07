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
#if TerrariaServer_V1_4

using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;
using System.Linq;

namespace OTAPI.Modifications
{
    [Modification(ModType.PostPatch, "Patching Netplay Broadcast")]
    [MonoMod.MonoModIgnore]
    class PatchNetplayBroadcast
    {
        public delegate bool NpcSpawnHandler(ref int index);

        const string Name_BroadcastThreadActive = nameof(Terraria.patch_Netplay.BroadcastThreadActive);
        const string Name_BroadcastThread = nameof(Terraria.patch_Netplay.orig_BroadcastThread);

        public PatchNetplayBroadcast(MonoModder modder)
        {
            var BroadcastThread = modder.GetDefinition<Terraria.Netplay>().Methods.Single(m => m.Name == Name_BroadcastThread).GetILCursor();
            var keepAlive = modder.GetDefinition<Terraria.Netplay>().Fields.Single(m => m.Name == Name_BroadcastThreadActive);

            BroadcastThread.GotoNext(
                i => i.OpCode == OpCodes.Call
                    && i.Operand is MethodReference methodReference
                    && methodReference.Name == "Sleep"
                    && i.Next.OpCode == OpCodes.Br_S
            );

            BroadcastThread.Index++;

            BroadcastThread.Emit(OpCodes.Ldsfld, keepAlive);
            BroadcastThread.Next.OpCode = OpCodes.Brtrue;
            BroadcastThread.Index++;
            BroadcastThread.Emit(OpCodes.Ret);
        }
    }
}
#endif
