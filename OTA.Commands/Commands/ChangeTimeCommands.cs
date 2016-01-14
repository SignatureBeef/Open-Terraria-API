using System;
using Terraria;
using OTA.Command;

namespace OTA.Commands
{
    public class ChangeTimeCommands : OTACommand
    {
        public override void Initialise()
        {
            AddCommand("dawn")
                .SetDefaultUsage()
                .WithDescription("Change time to dawn.")
                .ByPermissionNode("terraria.dawn")
                .Calls(Dawn);
            AddCommand("noon")
                .SetDefaultUsage()
                .WithDescription("Change time to noon.")
                .ByPermissionNode("terraria.noon")
                .Calls(Noon);
            AddCommand("dusk")
                .SetDefaultUsage()
                .WithDescription("Change time to dusk.")
                .ByPermissionNode("terraria.dusk")
                .Calls(Dusk);
            AddCommand("midnight")
                .SetDefaultUsage()
                .WithDescription("Change time to midnight.")
                .ByPermissionNode("terraria.midnight")
                .Calls(Midnight);
        }

        /// <summary>
        /// Changes the time to dawn
        /// </summary>
        /// <param name="sender">Sending entity</param>
        /// <param name="args">Arguments sent with command</param>
        public void Dawn(ISender sender, ArgumentList args)
        {
            Main.dayTime = true;
            Main.time = 0.0;
            NetMessage.SendData(7, -1, -1, String.Empty, 0, 0f, 0f, 0f, 0);

            sender.Message("Time set to dawn");
        }

        /// <summary>
        /// Changes the time to noon
        /// </summary>
        /// <param name="sender">Sending entity</param>
        /// <param name="args">Arguments sent with command</param>
        public void Noon(ISender sender, ArgumentList args)
        {
            Main.dayTime = true;
            Main.time = 27000.0;
            NetMessage.SendData(7, -1, -1, String.Empty, 0, 0f, 0f, 0f, 0);

            sender.Message("Time set to noon");
        }

        /// <summary>
        /// Changes the time to dusk
        /// </summary>
        /// <param name="sender">Sending entity</param>
        /// <param name="args">Arguments sent with command</param>
        public void Dusk(ISender sender, ArgumentList args)
        {
            Main.dayTime = false;
            Main.time = 0.0;
            NetMessage.SendData(7, -1, -1, String.Empty, 0, 0f, 0f, 0f, 0);

            sender.Message("Time set to dusk");
        }

        /// <summary>
        /// Changes the time to midnight
        /// </summary>
        /// <param name="sender">Sending entity</param>
        /// <param name="args">Arguments sent with command</param>
        public void Midnight(ISender sender, ArgumentList args)
        {
            Main.dayTime = false;
            Main.time = 16200.0;
            NetMessage.SendData(7, -1, -1, String.Empty, 0, 0f, 0f, 0f, 0);

            sender.Message("Time set to midnight");
        }
    }
}

