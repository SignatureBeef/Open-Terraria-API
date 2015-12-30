#if CLIENT
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace OTA.Client.Tile
{
    public class TileDefinition : TypeDefinition
    {
        public OTATile Tile { get; set; }
    }

    public class TileModRegister
    {
        const int MaxIds = Terraria.Main.maxTileSets;

        private readonly ConcurrentDictionary<String, TileDefinition> _entities = new ConcurrentDictionary<String, TileDefinition>();
        private int _nextId = MaxIds + 1;

        public static int MaxId = MaxIds + 1;

        public int Register<T>(string name) where T : OTATile
        {
            var def = new TileDefinition()
            {
                InstanceType = typeof(T),
                Tile = (OTATile)Activator.CreateInstance<T>()
            };
            if (_entities.TryAdd(name, def))
            {
                def.TypeId = System.Threading.Interlocked.Increment(ref _nextId);
                def.Tile.TypeId = (ushort)def.TypeId;

                if (MaxId < def.TypeId) MaxId = def.TypeId + 1;

                OTATile.ResizeArrays();

                return def.TypeId;
            }
            return 0;
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

        internal void InitialiseTiles()
        {
            foreach (var def in _entities)
                def.Value.Tile.Initialise();
        }
    }
}
#endif