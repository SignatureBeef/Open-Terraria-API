#if CLIENT
using System;
using OTA.Client.Npc;
using Terraria;

namespace OTA.Client.Test
{
    [NativeMod(SassyName)]
    public class Sassy : OTANpc
    {
        const string SassyName = "Sassy";

        public override void OnSetDefaults()
        {
            base.OnSetDefaults();

            EmulateNPC(Terraria.ID.NPCID.Merchant, true);

            this.IsTownNpc = true;

            this.name = SassyName;
            this.displayName = SassyName;
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
            Main.npcShop = 1; //TODO: shop hooks
            Main.instance.shop [Main.npcShop].SetupShop (Main.npcShop);
            Main.PlaySound (12, -1, -1, 1);
        }
        #endregion
    }
}
#endif