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
        private static IEnumerable<ModificationAttribute> Discover()
        {
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
                var modificationAttr = type.GetCustomAttribute<ModificationAttribute>();
                if (modificationAttr != null)
                {
                    modificationAttr.InstanceType = type;
                    yield return modificationAttr;
                }
            }
        }

        static void IterateMods(IEnumerable<ModificationAttribute> mods, Action<ModificationAttribute> action, MonoMod.MonoModder modder)
        {
            var queue = mods.ToDictionary(i => i, k => false);
            bool complete = queue.Count == 0;

            Func<ModificationAttribute, bool> areDepsCompleted = (mod) =>
            {
                if (mod.Dependencies != null)
                {
                    return mod.Dependencies.All(d => mods.Any(m => m.InstanceType == d && queue[m]));
                }
                return true;
            };

            do
            {
                foreach (var pair in queue
                    .Where(x => !x.Value)
                    .ToDictionary(k => k.Key, v => v.Value)
                )
                {
                    var is_ready = areDepsCompleted(pair.Key);
                    if (is_ready)
                    {
                        action(pair.Key);
                        queue[pair.Key] = true;
                    }
                    else modder.Log($"[OTAPI] Awaiting dependencies for {pair.Key.InstanceType.FullName}");
                }

                complete = queue.All(x => x.Value);
            } while (!complete);
        }

        public static void Apply(ModType modType, MonoMod.MonoModder modder)
        {
            modder.Log($"Processing {modType} OTAPI mods");
            var availableParameters = new List<object>()
            {
                modder,
                modType,
            };

            var modifications = Discover().Where(x => x.Type == modType);
            IterateMods(modifications, (modification) =>
            {
                modder.Log($"[OTAPI:{modType}] {modification.Description}");

                var modCtor = modification.InstanceType.GetConstructors().Single();
                var modCtorParams = modCtor.GetParameters();

                // bind arguments
                var args = new object[modCtorParams.Count()];
                {
                    for (var i = 0; i < modCtorParams.Count(); i++)
                    {
                        var param = modCtorParams.ElementAt(i);
                        var paramValue = availableParameters.SingleOrDefault(p =>
                            param.ParameterType.IsAssignableFrom(p.GetType())
                        );
                        if (paramValue != null)
                        {
                            args[i] = paramValue;
                        }
                        else throw new Exception($"No valid for parameter ${param.Name} in modification {modification.InstanceType.FullName}");
                    }
                }

                Activator.CreateInstance(modification.InstanceType, args, null);
            }, modder);
        }
    }
}
