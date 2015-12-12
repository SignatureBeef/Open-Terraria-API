#if CLIENT
using System;
using OTA.Client.Npc;
using Terraria;
using OTA.Client.Chest;
using OTA.Client.Item;

namespace OTA.Client.Test
{
    public class SassyFoodsShop : OTAShop
    {
        public override void OnInitialise(Terraria.Chest chest)
        {
            base.OnInitialise(chest);

            int i = 0;
            chest.item[i++].SetDefaults("Trippa Snippa");
            chest.item[i++].SetDefaults("Mining Helmet");
        }
    }

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

    [NativeMod(SassyName)]
    public class Sassy : OTANpc
    {
        const string SassyName = "Sassy";

        int shopId;

        public override void OnSetDefaults()
        {
            base.OnSetDefaults();

            EmulateNPC(Terraria.ID.NPCID.Merchant, true);

            Npc.IsTownNpc = true;

            Npc.name = SassyName;
            Npc.displayName = SassyName;

            shopId = EntityRegistrar.Shops.Register(new SassyFoodsShop());
        }

        public override string OnChat()
        {
            return ".....wadiyatalkinabeet";
        }

        public override string[] OnGetChatButtons()
        {
            /* Close is always in the middle  */
            return new string[] { "Sassy Foods" };
        }

        public override bool OnChatButtonClick(OTA.Callbacks.NpcChatButton button)
        {
            if (button == OTA.Callbacks.NpcChatButton.First)
            {
                OpenSassyFoods();
            }
            return base.OnChatButtonClick(button);
        }

        #region "Sassy foods"

        private void OpenSassyFoods()
        {
            Main.playerInventory = true;
            Main.npcChatText = string.Empty;
            Main.npcShop = shopId;
            Main.instance.shop[Main.npcShop].SetupShop(shopId);
            Main.PlaySound(12, -1, -1, 1);
        }

        #endregion
    }
}
#endif