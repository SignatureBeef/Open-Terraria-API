using System;
using OTA.Command;

namespace OTA.Commands
{
    public class SaveCommand : OTACommand
    {
        public override void Initialise()
        {
            AddCommand("save")
                .SetDefaultUsage()
                .WithDescription("Save the game world.")
                .WithPermissionNode("terraria.save")
                .Calls(Save);
        }

        public void Save(ISender sender, ArgumentList args)
        {
            Terraria.IO.WorldFile.saveWorld(false);
        }
    }
}

