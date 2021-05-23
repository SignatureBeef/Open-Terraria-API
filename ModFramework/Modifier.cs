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

using ModFramework.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ModFramework
{
    public static class Modifier
    {
        static void IterateMods(IEnumerable<ModificationAttribute> mods, Action<ModificationAttribute> action)
        {
            var queue = mods.ToDictionary(i => i, k => false);
            bool complete = queue.Count == 0;

            var emptyDeps = new Dictionary<string, bool>();
            Dictionary<string, bool> GetDependencyStatus(ModificationAttribute mod)
            {
                if (mod.Dependencies != null)
                {
                    return mod.Dependencies.ToDictionary(d => d, d => mods.Any(m => (m.MethodBase.DeclaringType.Name == d || (m.UniqueName != null && m.UniqueName == d)) && queue[m]));
                }
                return emptyDeps;
            }

            do
            {
                var tasks = queue
                    .Where(x => !x.Value)
                    .ToDictionary(k => k.Key, v => v.Value);
                foreach (var pair in tasks)
                {
                    var deps = GetDependencyStatus(pair.Key);
                    var not_ready = deps.Where(x => !x.Value);
                    var is_ready = !not_ready.Any();
                    if (is_ready)
                    {
                        action(pair.Key);
                        queue[pair.Key] = true;
                    }
                    else Console.WriteLine($"[ModFw] Awaiting dependencies for {pair.Key.MethodBase.DeclaringType.FullName} ({pair.Key.Description}) needs: {String.Join(",", not_ready.Select(x => x.Key))}");
                }

                complete = queue.All(x => x.Value);
            } while (!complete);
        }

        /* mainly to expose an Apply function without the need of MonoMod */
        public static void Apply(ModType modType) => Apply(modType, null, null);
        public static void Apply(ModType modType, IEnumerable<Assembly> assemblies) => Apply(modType, null, assemblies);
        public static void Apply(ModType modType, MonoMod.MonoModder modder) => Apply(modType, modder, null);

        public static void Apply(ModType modType, MonoMod.MonoModder modder, IEnumerable<Assembly> assemblies)
        {
            Console.WriteLine($"[ModFw:{modType}] Applying mods...");
            var availableParameters = new List<object>()
            {
                modType,
            };

            if (modder != null) availableParameters.Add(modder);

            var modifications = ModificationAttribute
                .Discover(assemblies ?? PluginLoader.Assemblies)
                .Where(x => x.Type == modType);
            IterateMods(modifications, (modification) =>
            {
                Console.WriteLine($"[ModFw:{modType}] {modification.Description}");

                MethodBase modCtor = modification.MethodBase; //.DeclaringType.GetConstructors().Single();
                var modCtorParams = modCtor.GetParameters();

                // bind arguments
                var args = new object[modCtorParams.Length];
                {
                    for (var i = 0; i < modCtorParams.Length; i++)
                    {
                        var param = modCtorParams[i];
                        var paramValue = availableParameters.SingleOrDefault(p =>
                            param.ParameterType.IsAssignableFrom(p.GetType())
                        );

                        if (paramValue != null)
                            args[i] = paramValue;
                        else throw new Exception($"No valid for parameter `{param.Name}` in modification {modification.MethodBase.DeclaringType.FullName}");
                    }
                }

                if (modification.MethodBase.IsConstructor)
                    Activator.CreateInstance(modification.MethodBase.DeclaringType, args, null);
                else modification.MethodBase.Invoke(modification.Instance, args);
            });
        }
    }
}
