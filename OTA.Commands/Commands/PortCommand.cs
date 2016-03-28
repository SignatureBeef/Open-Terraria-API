#if SERVER
using System;
using Terraria;
using OTA.Command;

namespace OTA.Commands
{
    public class PortCommand : OTACommand
    {
        public override void Initialise()
        {
            AddCommand("port")
                .SetDefaultUsage()
                .WithDescription("Print the listening port.")
                .WithPermissionNode("terraria.port")
                .Calls(Port);
        }

        public void Port(ISender sender, ArgumentList args)
        {
            sender.Message("Port: " + Netplay.ListenPort);
        }
    }
}
#endif