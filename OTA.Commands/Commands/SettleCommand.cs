using System;
using Terraria;
using OTA.Command;

namespace OTA.Commands
{
    public class SettleCommand : OTACommand
    {
        public override void Initialise()
        {
            AddCommand("settle")
                .SetDefaultUsage()
                .WithDescription("Settle all water.")
                .WithPermissionNode("terraria.settle")
                .Calls(Settle);
        }

        public void Settle(ISender sender, ArgumentList args)
        {
            if (!Liquid.panicMode)
            {
                if (sender is Player)
                    sender.Message("Forcing water to settle.");
                Liquid.StartPanic();
            }
            else
            {
                sender.Message("Water is already settling");
            }
        }
    }
}

