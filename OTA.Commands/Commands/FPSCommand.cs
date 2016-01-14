using System;
using Terraria;
using OTA.Command;

namespace OTA.Commands
{
    public class FPSCommand : OTACommand
    {
        public override void Initialise()
        {
            AddCommand("fps")
                .SetDefaultUsage()
                .WithDescription("Toggle FPS monitoring.")
                .ByPermissionNode("terraria.fps")
                .Calls(FPS);
        }

        public void FPS(ISender sender, ArgumentList args)
        {
            if (sender is ConsoleSender)
            {
                if (!Main.dedServFPS)
                {
                    Main.dedServFPS = true;
                    Main.fpsTimer.Reset();
                }
                else
                {
                    Main.dedServCount1 = 0;
                    Main.dedServCount2 = 0;
                    Main.dedServFPS = false;
                }
            }
            else
                throw new CommandError("This is a console only command");
        }
    }
}

