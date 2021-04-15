/*
Copyright (C) 2020 DeathCradle

This file is part of Open Terraria API v3 (OTAPI)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/
using System.Linq;

namespace OTAPI.Launcher
{
    static class Program
    {
        static void Main(string[] args)
        {
#if tModLoaderServer_V1_3
            Terraria.Program.SavePath = "ModLoader";
#endif
            On.Terraria.Program.LaunchGame += Program_LaunchGame;
            OTAPI.Hooks.MessageBuffer.ClientUUIDReceived = (@event, instance, reader, start, length, messageType) =>
            {
                if (@event == ModFramework.HookEvent.After)
                    System.Console.WriteLine($"ClientUUIDReceived {Terraria.Netplay.Clients[instance.whoAmI].ClientUUID}");

                return ModFramework.HookResult.Continue;
            };

            On.Terraria.WindowsLaunch.Main += WindowsLaunch_Main;

            // if testing (
            if (args.Any(x => x.ToLower() == "-test-init"))
                On.Terraria.Main.DedServ += Main_DedServ;

            Terraria.WindowsLaunch.Main(args);
        }

        private static void Program_LaunchGame(On.Terraria.Program.orig_LaunchGame orig, string[] args, bool monoArgs)
        {
            Terraria.Main.SkipAssemblyLoad = true;
            orig(args, monoArgs);
        }

        private static void Main_DedServ(On.Terraria.Main.orig_DedServ orig, Terraria.Main self)
        {
            System.Console.WriteLine($"Server init process successful");
        }

        private static void WindowsLaunch_Main(On.Terraria.WindowsLaunch.orig_Main orig, string[] args)
        {
            System.Console.WriteLine($"MonoMod runtime hooks active");
            orig(args); // now call the original Terraria.WindowsLaunch.Main instance
        }
    }
}
