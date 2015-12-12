using System;
using OTA.Client.Item;
using OTA.Client;

namespace BLS
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
            Item.maxStack = 30;
            Item.consumable = true;
            Item.width = 14;
            Item.height = 24;
            Item.potion = true;
            Item.value = 1500;
        }
    }
}

