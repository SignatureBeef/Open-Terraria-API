using System;
using Terraria;
using OTA.Command;

namespace OTA.Commands
{
    public class BanCommand : OTACommand
    {
        public override void Initialise()
        {
            AddCommand("ban")
                .WithDescription("Bans a player from the server.")
                .ByPermissionNode("terraria.ban")
                .Calls(Ban);
        }

        public void Ban(ISender sender, string player)
        {
            if (String.IsNullOrEmpty(player))
            {
                sender.Message("Usage: ban <player>");
            }
            else
            {
                bool found = false;
                var lowered = player.ToLower();
                for (int i = 0; i < 255; i++)
                {
                    if (Main.player[i].active && Main.player[i].name.ToLower() == lowered)
                    {
                        Terraria.Netplay.AddBan(i);
                        NetMessage.SendData(2, i, -1, "Banned from server.", 0, 0f, 0f, 0f, 0);
                        found = true;
                    }
                }
                if (!found)
                    sender.Message("Failed to find a player by the name of {0}", player);
            }
        }
    }
}

