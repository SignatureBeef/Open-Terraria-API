using System;
using OTA.Client.Npc;

namespace OTA.Client.Test
{
    [NativeMod("Sassy")]
    public class Sassy : OTANpc
    {
        public override void OnSetDefaults()
        {
            base.OnSetDefaults();

            EmulateNPC(Terraria.ID.NPCID.Guide, true);

            this.IsTownNpc = true;

            this.name = "Sassy";
            this.displayName = "Sassy";
        }

        public override string OnChat()
        {
            return ".....wadiyatalkinabeet";
        }
    }
}

