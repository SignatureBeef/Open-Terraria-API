using System;
using Terraria;
using OTA.Command;

namespace OTA.Commands
{
    public class MotdCommand : OTACommand
    {
        public override void Initialise()
        {
            AddCommand("motd")
                .WithDescription("Print or change the message of the day.")
                .ByPermissionNode("terraria.motd")
                .Calls(MOTD);
        }

        public void MOTD(ISender sender, string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                if (Main.motd == String.Empty)
                {
                    sender.Message("Welcome to " + Main.worldName + "!");
                }
                else
                {
                    sender.Message("MOTD: " + Main.motd);
                }
            }
            else
            {
                Main.motd = message;
            }
        }
    }
}

