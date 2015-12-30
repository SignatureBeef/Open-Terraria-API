#if CLIENT
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;

namespace OTA.Client.Item
{
    public class ItemModRegister
    {
        const int MaxItemIds = Terraria.Main.maxItemTypes;

        private readonly ConcurrentDictionary<String, TypeDefinition> _items = new ConcurrentDictionary<String, TypeDefinition>();

        private int _nextId = MaxItemIds + 1;

        public static int MaxItemId = MaxItemIds + 1;

        public int Register<T>(string name) where T : OTAItem
        {
            var def = new TypeDefinition()
            {
                InstanceType = typeof(T)
            };
            if (_items.TryAdd(name, def))
            {
                def.TypeId = System.Threading.Interlocked.Increment(ref _nextId);

                if (MaxItemId < def.TypeId) MaxItemId = def.TypeId + 1;

                Logging.ProgramLog.Debug.Log($"Current type: {def.TypeId}, Stored: " + _items[name].TypeId);

                OTAItem.ResizeArrays();

                return def.TypeId;
            }
            return 0;
        }

        public OTAItem Create(Terraria.Item item)
        {
            var mod = Create(item.type);
            if (mod != null)
            {
                mod.Item = item;
                item.Mod = mod;
            }
            return mod;
        }

        public OTAItem Create(int type)
        {
            var def = _items
                .Where(x => x.Value.TypeId == type)
                .Select(v => v.Value)
                .SingleOrDefault();
            if (def != null)
            {
                var mod = (OTAItem)Activator.CreateInstance(def.InstanceType);
                mod.TypeId = type;
                return mod;
            }
            return null;
        }

        public OTAItem Create(string name)
        {
            var def = Find(name);
            if (def != null)
            {
                var mod = (OTAItem)Activator.CreateInstance(def.InstanceType);
                mod.TypeId = def.TypeId;
                return mod;
            }
            return null;
        }

        public TypeDefinition Find(int type)
        {
            return _items.Where(x => x.Value.TypeId == type).Select(y => y.Value).FirstOrDefault();
        }

        public TypeDefinition Find(string name)
        {
            if (_items.ContainsKey(name)) return _items[name];
            return null;
        }

        public int this [string name]
        {
            get { return Find(name).TypeId; }
        }
    }
}
#endif