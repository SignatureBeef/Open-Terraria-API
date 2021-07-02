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
using ModFramework;
using Mono.Cecil.Cil;
using MonoMod;
using System.Linq;

/// <summary>
/// @doc Adds a Terraria.WorldGen.autoSave check in saveAndPlay
/// </summary>
[Modification(ModType.PreMerge, "Adding autoSave check to saveAndPlay")]
void SaveAndPlay(MonoModder modder)
{
    var vanilla = modder.GetILCursor(() => Terraria.WorldGen.saveAndPlay());
    var autoSave = modder.GetFieldDefinition(() => Terraria.Main.autoSave);

    var first_instruction = vanilla.Body.Instructions.First();
    vanilla.Goto(first_instruction);
    vanilla.Emit(OpCodes.Ldsfld, autoSave);
    vanilla.Emit(OpCodes.Brtrue, first_instruction);
    vanilla.Emit(OpCodes.Ret);
}