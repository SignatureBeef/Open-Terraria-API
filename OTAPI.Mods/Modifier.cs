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
using System.Reflection;

namespace OTAPI
{
    public static class Modifier
    {
        class Modification : ModificationAttribute
        {
            public Type InstanceType { get; set; }

            public Modification(
                ModificationType type,
                string description,
                ModificationPriority priority
            ) : base(type, description, priority) { }
        }

        private static IEnumerable<Modification> Discover()
        {
            var attr = typeof(ModificationAttribute);
            var asm = Assembly.GetExecutingAssembly();

            Type[] types;
            try
            {
                types = asm.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types;
            }

            var modificationTypes = types.Where(x => x != null && !x.IsAbstract);

            foreach (var type in modificationTypes)
            {
                var modificationAttr = type.CustomAttributes.SingleOrDefault(a => a.AttributeType == attr);
                if (modificationAttr != null)
                {
                    yield return new Modification(
                        (ModificationType)modificationAttr.ConstructorArguments[0].Value,
                        (string)modificationAttr.ConstructorArguments[1].Value,
                        (ModificationPriority)modificationAttr.ConstructorArguments[2].Value
                    )
                    {
                        InstanceType = type,
                    };
                }
            }
        }

        public static void Apply(ModificationType modificationType, MonoMod.MonoModder modder)
        {
            modder.Log($"Processing {modificationType} OTAPI mods");
            var availableParameters = new Dictionary<Type, object>()
            {
                { typeof(MonoMod.MonoModder), modder},
                { typeof(ModificationType), modificationType},
            };

            var modifications = Discover();
            foreach (var modification in modifications
                .Where(x => x.Type == modificationType)
                .OrderBy(x => x.Priority))
            {
                modder.Log($"[OTAPI] {modification.Description}");

                var modCtor = modification.InstanceType.GetConstructors().Single();
                var modCtorParams = modCtor.GetParameters();

                // bind arguments
                var args = new object[modCtorParams.Count()];
                {
                    for (var i = 0; i < modCtorParams.Count(); i++)
                    {
                        var param = modCtorParams.ElementAt(i);

                        if (availableParameters.TryGetValue(param.ParameterType, out object paramValue))
                        {
                            args[i] = paramValue;
                        }
                        else throw new Exception($"No valid for parameter ${param.Name} in modification {modification.InstanceType.FullName}");
                    }
                }

                Activator.CreateInstance(modification.InstanceType, args, null);
            }
        }
    }
}
