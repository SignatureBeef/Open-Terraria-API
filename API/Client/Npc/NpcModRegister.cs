#if CLIENT
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace OTA.Client.Npc
{
    public class NpcModRegister
    {
        const int MaxNpcIds = Terraria.Main.maxNPCTypes;

        private readonly ConcurrentDictionary<String, TypeDefinition> _npcs = new ConcurrentDictionary<String, TypeDefinition>();
        private int _nextId = MaxNpcIds + 1;

        public static int MaxNpcId = MaxNpcIds + 1;

        public int Register<T>(string name) where T : OTANpc
        {
            var def = new TypeDefinition()
            {
                InstanceType = typeof(T)
            };
            if (_npcs.TryAdd(name, def))
            {
                def.TypeId = System.Threading.Interlocked.Increment(ref _nextId);

                if (MaxNpcId < def.TypeId) MaxNpcId = def.TypeId + 1;

                OTANpc.ResizeArrays();

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
            return _npcs.Where(x => x.Value.TypeId == type).Select(y => y.Value).FirstOrDefault();
        }

        public TypeDefinition Find(string name)
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
#endif