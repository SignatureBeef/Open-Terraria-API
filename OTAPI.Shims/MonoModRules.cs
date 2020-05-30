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
                }

                foreach (var attr in new[]
                {
                    "MonoMod.AssemblyRedirectorAttribute",
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
                        asmref.Name = MonoModRule.Modder.Module.Assembly.Name.Name;
                        asmref.Version = MonoModRule.Modder.Module.Assembly.Name.Version;
                        asmref.PublicKey = MonoModRule.Modder.Module.Assembly.Name.PublicKey;
                        asmref.PublicKeyToken = MonoModRule.Modder.Module.Assembly.Name.PublicKeyToken;
                        // MonoModRule.Modder.Module.AssemblyReferences.Remove(asmref);
                    }
                }
            }
        }
    }
}
