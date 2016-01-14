using System;
using Terraria;
using OTA.Command;

namespace OTA.Commands
{
    public class MaxPlayersCommand : OTACommand
    {
        public override void Initialise()
        {
            AddCommand("maxplayers")
                .SetDefaultUsage()
                .WithDescription("Print the max number of players.")
                .ByPermissionNode("terraria.maxplayers")
                .Calls(MaxPlayers);
        }

        public void MaxPlayers(ISender sender, ArgumentList args)
        {
            sender.Message("Player limit: " + Main.maxNetPlayers);
        }
    }
}

