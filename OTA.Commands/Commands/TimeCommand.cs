using System;
using Terraria;
using OTA.Command;

namespace OTA.Commands
{
    public class TimeCommand : OTACommand
    {
        public override void Initialise()
        {
            AddCommand("time")
                .SetDefaultUsage()
                .WithDescription("Display game time.")
                .WithPermissionNode("terraria.time")
                .Calls(Time);
        }

        public void Time(ISender sender, ArgumentList args)
        {
            string text3 = "AM";
            double num = Main.time;
            if (!Main.dayTime)
            {
                num += 54000.0;
            }
            num = num / 86400.0 * 24.0;
            double num2 = 7.5;
            num = num - num2 - 12.0;
            if (num < 0.0)
            {
                num += 24.0;
            }
            if (num >= 12.0)
            {
                text3 = "PM";
            }
            int num3 = (int)num;
            double num4 = num - (double)num3;
            num4 = (double)((int)(num4 * 60.0));
            string text4 = string.Concat(num4);
            if (num4 < 10.0)
            {
                text4 = "0" + text4;
            }
            if (num3 > 12)
            {
                num3 -= 12;
            }
            if (num3 == 0)
            {
                num3 = 12;
            }
            sender.Message(string.Concat(new object[]
                {
                    "Time: ",
                    num3,
                    ":",
                    text4,
                    " ",
                    text3
                }));
        }
    }
}

