#if SERVER
using System;
using Terraria;
using OTA.Extensions;
using OTA.Command;

namespace OTA.Commands
{
    public class PlayingCommand : OTACommand
    {
        public override void Initialise()
        {
            AddCommand("playing")
                .SetDefaultUsage()
                .WithDescription("Shows the list of players.")
                .ByPermissionNode("terraria.playing")
                .Calls(Playing);
        }

        public void Playing(ISender sender, ArgumentList args)
        {
            var count = 0;
            for (int i = 0; i < 255; i++)
            {
                if (Main.player[i].active)
                {
                    count++;
                    sender.Message("{0} ({1})", Main.player[i].name, Netplay.Clients[i].RemoteAddress());
                }
            }
            if (count == 0)
                sender.Message("No players connected.");
            else if (count == 1)
                sender.Message("1 player connected.");
            else
                sender.Message(count + " players connected.");
        }
    }
}
#endif