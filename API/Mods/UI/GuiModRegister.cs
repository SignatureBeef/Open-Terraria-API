#if CLIENT
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;

namespace OTA.Mod.UI
{
    public class GuiModRegister
    {
        const int MaxIds = 20000; //Placeholder, this is not the actual value defined in Terraria.

        private readonly ConcurrentDictionary<Int32, OTAGui> _entities = new ConcurrentDictionary<Int32, OTAGui>();

        private int _nextId = MaxIds;

        public int Register<T>() where T : OTAGui
        {
            var next = System.Threading.Interlocked.Increment(ref _nextId);
            var instance = (OTAGui)Activator.CreateInstance<T>();
            if (_entities.TryAdd(next, instance))
            {
                instance.Initialise();
                return next;
            }
            return 0;
        }
    }
}
#endif