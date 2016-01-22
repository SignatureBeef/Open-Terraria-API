using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;

namespace OTA.Mod.Projectile
{
    public class ProjectileModRegister
    {
        const int MaxIds = Terraria.Main.maxProjectileTypes;

        private readonly ConcurrentDictionary<String, TypeDefinition> _entities = new ConcurrentDictionary<String, TypeDefinition>();

        private int _nextId = MaxIds;

        public static int MaxId = MaxIds;

        public int Register<T>(string name) where T : OTAProjectile
        {
            var def = new TypeDefinition()
            {
                InstanceType = typeof(T)
            };
            if (_entities.TryAdd(name, def))
            {
                def.TypeId = System.Threading.Interlocked.Increment(ref _nextId);

                if (MaxId < def.TypeId) MaxId = def.TypeId + 1;

                Logging.ProgramLog.Debug.Log($"Current type: {def.TypeId}, Stored: " + _entities[name].TypeId);

                OTAProjectile.ResizeArrays();

                return def.TypeId;
            }
            return 0;
        }

        public OTAProjectile Create(Terraria.Projectile projectile)
        {
            var mod = Create(projectile.type);
            if (mod != null)
            {
                mod.Projectile = projectile;
                projectile.Mod = mod;
            }
            return mod;
        }

        public OTAProjectile Create(int type)
        {
            var def = _entities
                .Where(x => x.Value.TypeId == type)
                .Select(v => v.Value)
                .SingleOrDefault();
            if (def != null)
            {
                var mod = (OTAProjectile)Activator.CreateInstance(def.InstanceType);
                mod.TypeId = type;
                return mod;
            }
            return null;
        }

        public OTAProjectile Create(string name)
        {
            var def = Find(name);
            if (def != null)
            {
                var mod = (OTAProjectile)Activator.CreateInstance(def.InstanceType);
                mod.TypeId = def.TypeId;
                return mod;
            }
            return null;
        }

        public TypeDefinition Find(int type)
        {
            return _entities.Where(x => x.Value.TypeId == type).Select(y => y.Value).FirstOrDefault();
        }

        public TypeDefinition Find(string name)
        {
            if (_entities.ContainsKey(name)) return _entities[name];
            return null;
        }

        public int this [string name]
        {
            get { return Find(name).TypeId; }
        }
    }
}