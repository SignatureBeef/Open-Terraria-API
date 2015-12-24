#if CLIENT
using System;
using Terraria;

namespace OTA.Client.Chest
{
    public abstract class OTAShop : INativeMod
    {
        public virtual void OnInitialise(Terraria.Chest chest)
        {
        }

        internal static void ResizeArrays()
        {
            //This is an expensive method so we are issuing blocks of space
            const Int32 BlockIssueSize = 50;

            if (EntityRegistrar.Shops.CurrentType + 1 >= Main.instance.shop.Length)
            {
                Array.Resize(ref Main.instance.shop, Main.instance.shop.Length + BlockIssueSize);

                for (var i = 0; i < Main.instance.shop.Length; i++)
                {
                    if (null == Main.instance.shop[i])
                        Main.instance.shop[i] = new Terraria.Chest();
                }
            }
        }
    }
}
#endif