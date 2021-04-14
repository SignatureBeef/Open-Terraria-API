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
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace ModFramework
{
    [MonoMod.MonoModIgnore]
    public static class MonoModExtensions
    {
        public static void RelinkAssembly(this MonoMod.MonoModder modder, ModuleDefinition source, ModuleDefinition target = null)
        {
            if (target is null) target = modder.Module;

            // register the mapping
            modder.RelinkModuleMap[source.Assembly.Name.Name] = target;

            // references are cleaned up in the fw modder after auto patching
        }

        public static void EmitAll(this ILCursor cursor, params object[] instructions)
        {
            foreach (var instruction in instructions)
            {
                var parsed = CecilHelpersExtensions.AnonymousToInstruction(instruction);
                cursor.Emit(parsed.OpCode, parsed.Operand);
            }
        }
    }
}
