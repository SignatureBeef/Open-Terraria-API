using System;
using OTA.Client.Item;
using OTA.Client;

namespace TBLS
{
    [NativeMod(TrippaSnippaName)]
    public class TrippaSnippa : OTAItem
    {
        public const string TrippaSnippaName = "Trippa Snippa";

        public override void OnSetDefaults()
        {
            LoadTexture("trippasnippa", true);

            Item.name = TrippaSnippaName;
            Item.healLife = 0;

            Item.useSound = 3;
            Item.useStyle = 2;
            Item.useTurn = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.maxStack = 2;
            Item.consumable = true;
            Item.width = 14;
            Item.height = 24;
            Item.potion = true;
//            Item.value = Terraria.Item.sellPrice(gold: 20);
            Item.value = Terraria.Item.sellPrice(copper: 1);
            Item.stack = 1;
        }
    }
}

