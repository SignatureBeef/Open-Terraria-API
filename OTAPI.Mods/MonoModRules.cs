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
using Mono.Cecil;
using MonoMod.InlineRT;
using System;
using System.Linq;

namespace MonoMod
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class FieldsToPropertyTransformerAttribute : Attribute
    {
        /// <summary>
        /// Transforms all fields to properties
        /// </summary>
        public FieldsToPropertyTransformerAttribute() { }
    }

    static class MonoModRules
    {
        static MonoModRules()
        {
            MonoModRule.Modder.Log($"Processing isolated OTAPI mods");
            var mods = MonoModRule.Modder.Mods.Cast<ModuleDefinition>();
            foreach (var mod in mods)
            {
                foreach (var type in mod.Types)
                {
                    ScanType(mod, type);
                }

                foreach (var attr in new[]
                {
                    "MonoMod.FieldsToPropertyTransformerAttribute",
                })
                {
                    var type = mod.GetType(attr);
                    if (type != null)
                    {
                        mod.Types.Remove(type);
                    }
                }
            }
        }

        static void ScanType(ModuleDefinition mod, TypeDefinition type)
        {
            TryProcessFieldToPropertyTransformers(mod, type);

            if (type.HasNestedTypes)
            {
                foreach (var nested in type.NestedTypes)
                    ScanType(mod, nested);
            }
        }

        static void TryProcessFieldToPropertyTransformers(ModuleDefinition mod, TypeDefinition type)
        {
            foreach (var attr in type.CustomAttributes.ToArray())
            {
                if (attr.AttributeType.FullName == "MonoMod.FieldsToPropertyTransformerAttribute")
                {
                    MonoModRule.Modder.Log($"Transforming types fields to properties: {type.BaseType.FullName}");
                    type.CustomAttributes.Remove(attr);
                }
            }
        }
    }
}
