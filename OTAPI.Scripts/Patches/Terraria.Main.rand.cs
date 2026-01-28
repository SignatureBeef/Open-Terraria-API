/*
Copyright (C) 2025 sors89

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
#if !tModLoader && !tModLoaderServer
using ModFramework;
using Mono.Cecil.Cil;
using MonoMod;
using System;
using System.Linq;

/// <summary>
/// @doc Patch Terraria.Main.rand into a property
/// </summary>
[MonoModIgnore]
partial class Rand
{
    [Modification(ModType.PreMerge, "Patching Terraria.Main.rand")]
    static void PatchMainRand(ModFwModder modder)
    {
        var rand = modder.GetFieldDefinition(() => Terraria.Main.rand);
        var prop = rand.RemapAsProperty(modder);

        var csr = modder.GetILCursor(prop.GetMethod);

        /*
        0   0000    ldsfld  string Terraria.Main::'<rand>k__BackingField'
        1   0005    ret
        */

        var backingField = modder.Module.GetDefinition<Terraria.Main>().Fields.Where(x => x.Name.Contains("<rand>")).First();

        csr.GotoNext(i => i.OpCode == OpCodes.Ret);
        csr.Emit(OpCodes.Ldnull);
        csr.Emit(OpCodes.Nop);
        csr.Emit(OpCodes.Newobj, typeof(Terraria.Utilities.UnifiedRandom).GetConstructor(new Type[] { }));
        csr.Emit(OpCodes.Stsfld, backingField);
        csr.Emit(OpCodes.Ldsfld, backingField);

        var nop = csr.Body.Instructions.Where(i => i.OpCode == OpCodes.Nop).First();
        nop.OpCode = OpCodes.Bne_Un_S;
        nop.Operand = csr.Previous;
    }
}
#endif