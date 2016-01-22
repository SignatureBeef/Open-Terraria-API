using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;

namespace OTA.Mod.Npc
{
    public class NpcModRegister
    {
        const int MaxNpcIds = Terraria.Main.maxNPCTypes;

        private readonly ConcurrentDictionary<String, TypeDefinition> _entities = new ConcurrentDictionary<String, TypeDefinition>();
        private readonly List<OTANpc> _instances = new List<OTANpc>();

        private int _nextId = MaxNpcIds;

        public static int MaxNpcId = MaxNpcIds;

        public int Register<T>(string name) where T : OTANpc
        {
            var def = new TypeDefinition()
            {
                InstanceType = typeof(T)
            };
            if (_entities.TryAdd(name, def))
            {
                def.TypeId = System.Threading.Interlocked.Increment(ref _nextId);

                if (MaxNpcId < def.TypeId) MaxNpcId = def.TypeId + 1;

                OTANpc.ResizeArrays();

                //Create a non-npc related definition in order for us to call simple non-instance events
                var npc = (OTANpc)Activator.CreateInstance<T>();
                npc.TypeId = def.TypeId;
                _instances.Add(npc);

                return def.TypeId;
            }
            return 0;
        }

        public OTANpc Create(Terraria.NPC npc)
        {
            var mod = Create(npc.type);
            if (mod != null)
            {
                mod.Npc = npc;
                npc.Mod = mod;
            }
            return mod;
        }

        public OTANpc Create(int type)
        {
            var def = Find(type);
            if (def != null)
            {
                var mod = (OTANpc)Activator.CreateInstance(def.InstanceType);
                mod.TypeId = type;
                return mod;
            }
            return null;
        }

        public OTANpc Create(string name)
        {
            var def = Find(name);
            if (def != null)
            {
                var mod = (OTANpc)Activator.CreateInstance(def.InstanceType);
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

        public int this[string name]
        {
            get { return Find(name).TypeId; }
        }

        internal IEnumerable<T> Select<T>(Func<OTANpc, T> ex)
        {
            return _instances.Select(ex);
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