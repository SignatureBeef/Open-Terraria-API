#if CLIENT
using System;
using System.Collections.Concurrent;

namespace OTA.Client.Chest
{
    public class ShopModRegister
    {
        private int _type = Terraria.Main.numShops + 1;

        private readonly ConcurrentDictionary<Int32, OTAShop> _shops = new ConcurrentDictionary<Int32, OTAShop>();

        public int CurrentType
        {
            get { return _type; }
        }

        public int Register(OTAShop shop)
        {
            var id = System.Threading.Interlocked.Increment(ref _type);

            if (_shops.TryAdd(id, shop))
            {
                OTAShop.ResizeArrays();
                return id;
            }

            return 0;
        }

        public OTAShop Find(int type)
        {
            if (_shops.ContainsKey(type)) return _shops[type];
            return null;
        }
    }
}
#endif