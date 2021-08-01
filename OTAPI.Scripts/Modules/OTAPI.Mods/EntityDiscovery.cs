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
#pragma warning disable CS0436 // Type conflicts with imported type

using ModFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OTAPI.Mods
{
    //public class EntityMod
    //{
    //    public Type Type { get; set; }
    //    public EntityModAttribute Attribute { get; set; }
    //}

    public static class EntityDiscovery
    {
        private static Dictionary<Type, List<IMod>> _mods = Discover();

        [Modification(ModType.Runtime, "Loading entity mod interface")]
        public static void OnBoot(Assembly runtimeAssembly)
        {
            var line = String.Join(", ", _mods.Keys.Select(x => $"{x}: {_mods[x].Count}"));
            Console.WriteLine($"Loaded mods: {line}");
        }

        public static Dictionary<Type, List<IMod>> Discover()
        {
            var mods = new Dictionary<Type, List<IMod>>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .Select(a => new
                {
                    Assembly = a,
                    Types = a.GetAssemblyTypes(),
                });

            foreach (var assembly in assemblies)
            {
                var mod_types = assembly.Types.Where(t => typeof(IMod).IsAssignableFrom(t) && t.BaseType is not null);

                foreach (var mod_type in mod_types)
                {
                    var baseType = mod_type.BaseType!;

                    if (!mods.TryGetValue(baseType, out List<IMod>? entityMods))
                    {
                        entityMods = new List<IMod>();
                        mods.Add(baseType, entityMods);
                    }

                    var instance =  Activator.CreateInstance(mod_type);
                    if(instance is IMod mod)
                    {
                        entityMods.Add(mod);
                    }
                    else
                    {
                        Console.WriteLine($"[OTAPI] Failed to load mod type: {mod_type.FullName}");
                    }
                }
            }

            return mods;
        }

        internal static IEnumerable<Type> GetAssemblyTypes(this Assembly assembly)
        {
            try
            {
                return assembly.GetExportedTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return (IEnumerable<Type>)(ex.Types?
                    .Where(x => x is not null)
                    ?? Enumerable.Empty<Type>());
            }
        }

        public static IEnumerable<IMod> GetTypeMods<TMod>() where TMod : IMod
        {
            if (_mods.TryGetValue(typeof(TMod), out List<IMod>? mods)) return mods;
            return Enumerable.Empty<IMod>();
        }

        public static void AddEntityMod<TMod>(IMod mod) where TMod : IMod
        {
            if (_mods.TryGetValue(typeof(TMod), out List<IMod>? mods))
            {
                mods.Add(mod);
            }
            else
            {
                _mods.Add(typeof(TMod), new List<IMod>()
                {
                    mod
                });
            }
        }
    }
}
