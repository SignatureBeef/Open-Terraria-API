using System;
using OTA.Client.Npc;
using OTA.Client;
using Terraria;

namespace BLS
{
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

