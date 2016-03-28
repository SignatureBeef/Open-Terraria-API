#if SERVER
using System;
using Terraria;
using OTA.Command;

namespace OTA.Commands
{
    public class PasswordCommand : OTACommand
    {
        public override void Initialise()
        {
            AddCommand("password")
                .WithDescription("Shows or changes to password.")
                .WithPermissionNode("terraria.password")
                .Calls(Password);
        }

        public void Password(ISender sender, string password)
        {
            if (String.IsNullOrEmpty(Netplay.ServerPassword))
            {
                if (String.IsNullOrEmpty(password))
                {
                    sender.Message("No password set.");
                }
                else
                {
                    Netplay.ServerPassword = password;
                    sender.Message("Password: " + Netplay.ServerPassword);
                }
            }
            else
            {
                if (String.IsNullOrEmpty(password))
                {
                    Netplay.ServerPassword = String.Empty;
                    sender.Message("Password disabled.");
                }
                else
                {
                    Netplay.ServerPassword = password;
                    sender.Message("Password: " + Netplay.ServerPassword);
                }
            }
        }
    }
}
#endif