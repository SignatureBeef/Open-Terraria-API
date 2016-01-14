using System;
using Terraria;
using OTA.Command;

namespace OTA.Commands
{
    public class KickCommand : OTACommand
    {
        public override void Initialise()
        {
            AddCommand("kick")
                .WithDescription("Kicks a player from the server.")
                .ByPermissionNode("terraria.kick")
                .Calls(Kick);
        }

        public void Kick(ISender sender, string player)
        {
            if (String.IsNullOrEmpty(player))
            {
                sender.Message("Usage: kick <player>");
            }
            else
            {
                bool found = false;
                var lowered = player.ToLower();
                for (int i = 0; i < 255; i++)
                {
                    if (Main.player[i].active && Main.player[i].name.ToLower() == lowered)
                    {
                        NetMessage.SendData(2, i, -1, "Kicked from server.", 0, 0f, 0f, 0f, 0);
                        found = true;
                    }
                }
                if (!found)
                    sender.Message("Failed to find a player by the name of {0}", player);
            }
        }
    }
}

