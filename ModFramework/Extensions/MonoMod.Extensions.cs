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
using System.Collections.Generic;
using System.Linq;
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

        public static IEnumerable<Instruction> EmitAll(this ILCursor cursor, params object[] instructions)
        {
            var emitted = new List<Instruction>();
            foreach (var instruction in instructions)
            {
                var parsed = CecilHelpersExtensions.AnonymousToInstruction(instruction);
                var csr = cursor.Emit(parsed.OpCode, parsed.Operand);
                emitted.Add(csr.Prev);
            }
            return emitted;
        }

        public static void MakeVirtual<TType>(this ModFwModder modder)
        {
            var type = modder.GetDefinition<TType>();
            modder.MakeVirtual(type);
        }

        public static void MakeVirtual(this ModFwModder modder, TypeDefinition type)
        {
            var methods = type.Methods.Where(m => !m.IsConstructor && !m.IsStatic).ToArray();
            foreach (var method in methods)
            {
                method.IsVirtual = true;
                method.IsNewSlot = true;
                method.IsFinal = false;
            }

            modder.OnRewritingMethodBody += (MonoMod.MonoModder modder, MethodBody body, Instruction instr, int instri) =>
            {
                if (methods.Any(x => x == instr.Operand))
                {
                    if (instr.OpCode != OpCodes.Callvirt)
                        instr.OpCode = OpCodes.Callvirt;
                }
            };
        }

        public static void RemoveWhile(this ILCursor cursor, Func<Instruction, bool> condition, bool removeAfterCondition = true)
        {
            while (condition(cursor.Next))
                cursor.Remove();

            if (removeAfterCondition)
                cursor.Remove();
        }

        public static CustomAttribute CreateAddMetadataAttribute(this ModFwModder mm, string key, string value)
        {
            var sac = mm.Module.ImportReference(typeof(System.Reflection.AssemblyMetadataAttribute).GetConstructor(new[] {
                typeof(string),
                typeof(string),
            }));
            var sa = new CustomAttribute(sac);
            sa.ConstructorArguments.Add(new CustomAttributeArgument(mm.Module.TypeSystem.String, key));
            sa.ConstructorArguments.Add(new CustomAttributeArgument(mm.Module.TypeSystem.String, value));
            return sa;
        }

        public static void AddMetadata(this ModFwModder mm, string key, string value, bool replace = true)
        {
            var existing = mm.GetMetadata(key);
            if (existing.Any())
            {
                foreach (var match in existing.ToArray())
                {
                    mm.Module.Assembly.CustomAttributes.Remove(match);
                }
                var sa = mm.CreateAddMetadataAttribute(key, value);
                mm.Module.Assembly.CustomAttributes.Add(sa);
            }
            else
            {
                var sa = mm.CreateAddMetadataAttribute(key, value);
                mm.Module.Assembly.CustomAttributes.Add(sa);
            }
        }

        public static IEnumerable<CustomAttribute> GetMetadata(this ModFwModder mm, string key)
        {
            var matches = mm.Module.Assembly.CustomAttributes
                .Where(ca => ca.Constructor.DeclaringType.Name == nameof(System.Reflection.AssemblyMetadataAttribute)
                    && ca.ConstructorArguments.Count == 2
                    && ca.ConstructorArguments[0].Type == mm.Module.TypeSystem.String
                    && ca.ConstructorArguments[1].Type == mm.Module.TypeSystem.String
                    && ca.ConstructorArguments[0].Value.Equals(key)
                )
                .ToArray();

            return matches;
        }

        public static IEnumerable<string> GetMetadataValues(this ModFwModder mm, string key)
            => mm.GetMetadata(key).Select(ca => (string)ca.ConstructorArguments[1].Value);
    }
}
