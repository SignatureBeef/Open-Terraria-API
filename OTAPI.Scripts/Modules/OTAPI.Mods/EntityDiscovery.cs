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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OTAPI.Mods
{
    public class EntityMod
    {
        public Type Type { get; set; }
        public EntityModAttribute Attribute { get; set; }
    }

    public static class Registry
    {

    }

    public static class EntityDiscovery
    {
        private static Dictionary<Type, List<EntityMod>> _mods = Discover();

        public static Dictionary<Type, List<EntityMod>> Discover()
        {
            var mods = new Dictionary<Type, List<EntityMod>>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .Select(a => new
                {
                    Assembly = a,
                    Types = a.GetAssemblyTypes(),
                });

            foreach (var assembly in assemblies)
            {
                var attrs = assembly.Types.Select(t => new
                {
                    Type = t,
                    Attribute = t.CustomAttributes?.FirstOrDefault(att => att.AttributeType == typeof(EntityModAttribute))
                }).Where(v => v.Attribute is not null);

                foreach (var attr in attrs)
                {
                    var baseType = attr.Type.BaseType;

                    if (!mods.TryGetValue(baseType, out List<EntityMod> entityMods))
                    {
                        entityMods = new List<EntityMod>();
                        mods.Add(baseType, entityMods);
                    }

                    var instance = (EntityModAttribute)attr.Attribute.Constructor.Invoke(attr.Attribute.ConstructorArguments.Select(x => x.Value).ToArray());

                    foreach (var na in attr.Attribute.NamedArguments)
                    {
                        typeof(EntityModAttribute).GetProperty(na.MemberName).SetValue(instance, na.TypedValue);
                    }

                    entityMods.Add(new EntityMod()
                    {
                        Attribute = instance,
                        Type = attr.Type
                    });
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

        public static IEnumerable<EntityMod> GetTypeMods<TMod>() where TMod : IMod
        {
            if (_mods.TryGetValue(typeof(TMod), out List<EntityMod> mods)) return mods;
            return Enumerable.Empty<EntityMod>();
        }
    }
}
