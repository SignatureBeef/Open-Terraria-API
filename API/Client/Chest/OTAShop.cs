#if CLIENT
using System;

namespace OTA.Client.Chest
{
    public abstract class OTAShop : INativeMod
    {
        public virtual void OnInitialise(Terraria.Chest chest)
        {
        }
    }
}
#endif