using System;
using OTA.Client.Chest;

namespace BLS
{
    public class SassyFoodsShop : OTAShop
    {
        public override void OnInitialise(Terraria.Chest chest)
        {
            base.OnInitialise(chest);

            int i = 0;
            chest.item[i++].SetDefaults("Trippa Snippa");
        }
    }
}

