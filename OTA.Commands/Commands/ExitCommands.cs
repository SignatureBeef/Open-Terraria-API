using System;
using Terraria;
using OTA.Command;

namespace OTA.Commands
{
    public class ExitCommands : OTACommand
    {
        public override void Initialise()
        {
            AddCommand("exit")
                .SetDefaultUsage()
                .WithDescription("Shutdown the server and save.")
                .ByPermissionNode("terraria.exit")
                .Calls(Exit);
            
            AddCommand("exit-nosave")
                .SetDefaultUsage()
                .WithDescription("Shutdown the server without saving.")
                .ByPermissionNode("terraria.exit-nosave")
                .Calls(ExitNoSave);
        }

        /// <summary>
        /// Saves world then exits server.
        /// </summary>
        /// <param name="sender">Sending entity</param>
        /// <param name="args">Arguments sent with command</param>
        public void Exit(ISender sender, ArgumentList args)
        {
            Tools.NotifyAllOps("Exiting...");

            Terraria.IO.WorldFile.saveWorld(false);
            Netplay.disconnect = true;
        }

        /// <summary>
        /// Exits server without first saving the world.
        /// </summary>
        /// <param name="sender">Sending entity</param>
        /// <param name="args">Arguments sent with command</param>
        public void ExitNoSave(ISender sender, ArgumentList args)
        {
            Tools.NotifyAllOps("Exiting without saving...");
            Netplay.disconnect = true;
        }
    }
}