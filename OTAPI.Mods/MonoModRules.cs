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
    //[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    //public class ChangeArchitectureAttribute : Attribute
    //{
    //    public ChangeArchitectureAttribute(TargetArchitecture architecture, ModuleAttributes attributes) { }
    //}

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public class AssemblyRedirectorAttribute : Attribute
    {
        /// <summary>
        /// Forwards all references to an assembly
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="targetAssemblyName">The target assembly name to forward to (not fully qualified). Defaults to the source assembly.</param>
        public AssemblyRedirectorAttribute(string assemblyName, string targetAssemblyName = null) { }
    }

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
                foreach (var attr in mod.GetCustomAttributes())
                {
                    TryProcessRedirectors(mod, attr);
                    //TryProcessArchitecture(mod, attr);
                }

                foreach (var type in mod.Types)
                {
                    ScanType(mod, type);
                }

                foreach (var attr in new[]
                {
                    "MonoMod.AssemblyRedirectorAttribute",
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

        static void TryProcessRedirectors(ModuleDefinition mod, CustomAttribute attr)
        {
            if (attr.AttributeType.FullName == "MonoMod.AssemblyRedirectorAttribute")
            {
                string assemblyNamePattern = (string)attr.ConstructorArguments[0].Value;
                string targetAssemblyName = (string)attr.ConstructorArguments[1].Value;

                var targetAssembly = targetAssemblyName ?? mod.Name;

                MonoModRule.RelinkModule(assemblyNamePattern, targetAssembly);

                foreach (var asmref in MonoModRule.Modder.Module.AssemblyReferences.ToArray())
                {
                    if (asmref.Name.Equals(assemblyNamePattern))
                    {
                        MonoModRule.Modder.Module.AssemblyReferences.Remove(asmref);
                    }
                }
            }
        }

        //static void TryProcessArchitecture(ModuleDefinition mod, CustomAttribute attr)
        //{
        //    if (attr.AttributeType.FullName == "MonoMod.ChangeArchitectureAttribute")
        //    {
        //        TargetArchitecture targetArchitecture = (TargetArchitecture)attr.ConstructorArguments[0].Value;
        //        ModuleAttributes moduleAttributes = (ModuleAttributes)attr.ConstructorArguments[1].Value;

        //        MonoModRule.Modder.Module.Architecture = targetArchitecture;
        //        MonoModRule.Modder.Module.Attributes = moduleAttributes;

        //        MonoModRule.Modder.Log($"Changed architecture {targetArchitecture} {moduleAttributes}");
        //    }
        //}

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
