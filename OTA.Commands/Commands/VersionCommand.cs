using System;
using Terraria;
using OTA.Command;

namespace OTA.Commands
{
    public class VersionCommand : OTACommand
    {
        public override void Initialise()
        {
            AddCommand("version")
                .SetDefaultUsage()
                .WithDescription("Print version number.")
                .ByPermissionNode("terraria.version")
                .Calls(Version);
        }

        public void Version(ISender sender, ArgumentList args)
        {
            sender.Message("Terraria Server " + Main.versionNumber);
            sender.Message("OTA Version " + Globals.BuildInfo);
        }
    }
}

