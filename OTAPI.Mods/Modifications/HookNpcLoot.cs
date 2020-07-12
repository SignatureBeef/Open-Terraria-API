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
using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;

namespace OTAPI.Modifications
{
    [Modification(ModType.PreMerge, "Hooking Terraria.GameContent.ItemDropRules.CommonCode.DropItemFromNPC")]
    [MonoMod.MonoModIgnore]
    class HookNpcLoot
    {
        public delegate bool NpcSpawnHandler(ref int index);

        public HookNpcLoot(MonoModder modder)
        {
            var NewNPC = modder.GetILCursor(() => Terraria.GameContent.ItemDropRules.CommonCode.DropItemFromNPC(default, default, default, default));

            NewNPC.GotoNext(
                i => i.OpCode == OpCodes.Call && i.Operand is MethodReference methodReference && methodReference.Name == "NewItem" && methodReference.DeclaringType.FullName == "Terraria.Item"
            );

            NewNPC.Emit(OpCodes.Ldarg_0); // NPC instance
            NewNPC.Next.Operand = modder.GetMethodDefinition(() => Callbacks.NPC.DropLoot(default, default, default, default, default, default, default, default, default, default, default));
        }
    }
}
#endif