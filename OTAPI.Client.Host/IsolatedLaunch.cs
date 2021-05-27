// Copyright (C) 2020-2021 DeathCradle
//
// This file is part of Open Terraria API v3 (OTAPI)
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
using System;
namespace OTAPI.Client
{
    public class IsolatedLaunch
    {
        public static void Launch(string[] args)
        {
            //On.Terraria.Program.LaunchGame += Program_LaunchGame;
            Terraria.Program.OnLaunched += (_, _) =>
            {
                Console.WriteLine("Launched");

                Terraria.Main.versionNumber += " [OTAPI.Client]";
                Terraria.Main.versionNumber2 += " [OTAPI.Client]";

                using (var lua = new Triton.Lua())
                {
                    lua.ImportNamespace("Terraria");
                    lua.DoString(@"
                        Main.versionNumber = Main.versionNumber .. ' Hellow from LUA'
                    ");
                }
            };
            Terraria.MacLaunch.Main(args);
        }

        //private static void Program_LaunchGame(On.Terraria.Program.orig_LaunchGame orig, string[] args, bool monoArgs)
        //{
        //    Console.WriteLine("Launched");
        //    orig(args, monoArgs);
        //}
    }
}
