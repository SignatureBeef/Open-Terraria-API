using System;
using System.Collections.Concurrent;
using System.Linq;

namespace OTA.Client.Npc
{
    public class NpcModRegister
    {
        const int MaxNpcIds = Terraria.Main.maxNPCTypes;

        private readonly ConcurrentDictionary<String, NpcDefinition> _npcs = new ConcurrentDictionary<String, NpcDefinition>();
        private int _nextId = MaxNpcIds + 1;

        public static int MaxNpcId = MaxNpcIds + 1;

        public int Register<T>(string name) where T : OTANpc
        {
            var def = new NpcDefinition()
                {
                    InstanceType = typeof(T)
                };
            if (_npcs.TryAdd(name, def))
            {
                def.TypeId = System.Threading.Interlocked.Increment(ref _nextId);

                if (MaxNpcId < def.TypeId) MaxNpcId = def.TypeId + 1;

                return def.TypeId;
            }
            return 0;
        }

        public OTANpc Create(int type)
        {
            var npc = _npcs
                .Where(x => x.Value.TypeId == type)
                .Select(v => v.Value)
                .SingleOrDefault();
            if (npc != null) return (OTANpc)Activator.CreateInstance(npc.InstanceType);
            return null;
        }

        public NpcDefinition Find(int type)
        {
            return _npcs.Where(x => x.Value.TypeId == type).Select(y => y.Value).FirstOrDefault();
        }

        public NpcDefinition Find(string name)
        {
            if (_npcs.ContainsKey(name)) return _npcs[name];
            return null;
        }

        public int this [string name]
        {
            get { return Find(name).TypeId; }
        }

        //        public NpcDef this [string name]
        //        {
        //            get { return Find(name); }
        //        }

        //        public NpcDef this [int type]
        //        {
        //            get { return Find(type); }
        //        }

        //        public int TypeId(string name)
        //        {
        //            return Find(name).TypeId;
        //        }
    }
}

