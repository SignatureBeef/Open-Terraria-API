using System;
using Terraria;
using OTA.Command;

namespace OTA.Commands
{
    public class ClearCommand : OTACommand
    {
        public override void Initialise()
        {
            AddCommand("clear")
                .SetDefaultUsage()
                .WithDescription("Clear the console window.")
                .WithPermissionNode("terraria.clear")
                .Calls(Clear);
        }

        public void Clear(ISender sender, ArgumentList args)
        {
            if (sender is ConsoleSender)
            {
                Console.Clear();
            }
            else
                sender.Message("clear: This is a console only command");
        }
    }
}