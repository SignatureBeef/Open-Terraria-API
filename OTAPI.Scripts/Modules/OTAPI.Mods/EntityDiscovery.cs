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
using ReLogic.Content.Sources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OTAPI.Mods
{
    public class EntityDiscovery
    {
        private Dictionary<Type, List<IMod>> _mods;

        public static EntityDiscovery Instance = new EntityDiscovery();

        [Modification(ModType.Runtime, "Loading entity mod interface")]
        public static void OnBoot(Assembly runtimeAssembly)
        {
            On.Terraria.Main.LoadContent += Main_LoadContent;
        }

        private static void Main_LoadContent(On.Terraria.Main.orig_LoadContent orig, Terraria.Main self)
        {
            orig(self);

            Instance.Discover();

            var line = String.Join(", ", Instance._mods.Keys.Select(x => $"{x}: {Instance._mods[x].Count}"));
            Console.WriteLine($"Loaded mods: {line}");
        }

        public EntityDiscovery()
        {
            _mods = new Dictionary<Type, List<IMod>>();
        }

        void Discover()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .Select(a => new
                {
                    Assembly = a,
                    Types = GetAssemblyTypes(a),
                });

            foreach (var assembly in assemblies)
            {
                var mod_types = assembly.Types.Where(t => typeof(IMod).IsAssignableFrom(t) && t.BaseType is not null);

                foreach (var mod_type in mod_types)
                {
                    var baseType = mod_type.BaseType!;

                    // skip the Mod type base
                    if (baseType == typeof(object))
                        continue;

                    var instance = Activator.CreateInstance(mod_type);
                    if (instance is IMod mod)
                    {
                        Register(baseType, mod);
                    }
                    else
                    {
                        Console.WriteLine($"[OTAPI] Failed to load mod type: {mod_type.FullName}");
                    }
                }
            }
        }

        static IEnumerable<Type> GetAssemblyTypes(Assembly assembly)
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

        public IEnumerable<TMod> GetTypeModRegistrations<TMod>() where TMod : IMod
        {
            if (_mods.TryGetValue(typeof(TMod), out List<IMod>? mods)) return mods.Cast<TMod>();
            return Enumerable.Empty<TMod>();
        }

        public void AddEntityMod<TMod>(TMod mod) where TMod : IMod
        {
            Type regoType;
            var modType = typeof(TMod);
            if (typeof(IMod).IsAssignableFrom(modType.BaseType))
                regoType = modType.BaseType;
            else if (typeof(IMod).IsAssignableFrom(modType))
                regoType = modType;
            else throw new Exception($"The base type of ${modType.FullName} does not extend from {typeof(IMod).FullName}");

            Register(regoType, mod);
        }

        Dictionary<string, IContentSource> ModSources = new Dictionary<string, IContentSource>();

        private void Register(Type entityType, IMod regoInstance)
        {
            if (!_mods.TryGetValue(entityType, out List<IMod>? entityMods))
            {
                entityMods = new List<IMod>();
                _mods.Add(entityType, entityMods);
            }

            entityMods.Add(regoInstance);

            var modName = regoInstance.GetType().Assembly.GetName().Name.Replace("CSharpScript_", "");
            if (!String.IsNullOrWhiteSpace(modName) && !ModSources.TryGetValue(modName, out var source))
            {
                var resources = Path.Combine("modifications", modName, "Resources");
                if (Directory.Exists(resources))
                {
                    Console.WriteLine($"[{regoInstance.Name.Key}] Using resources {resources}");
                    source = new FileSystemContentSource(resources);
                    Terraria.Main.AssetSourceController._staticSources.Add(source);
                    ModSources[modName] = source;
                }
                else Console.WriteLine($"[{regoInstance.Name.Key}] No resources found at {resources ?? "<null>"}");
            }
            else Console.WriteLine($"[{regoInstance.Name.Key}] No resources found for assembly {modName}");

            regoInstance.Registered();
        }
    }
}
