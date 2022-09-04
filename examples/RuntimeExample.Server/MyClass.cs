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

using System;
using ModFramework;

namespace RuntimeExample.Server;

public static class Hooks
{
    [Modification(ModType.Runtime, "Loading Runtime Example!")]
    public static void OnRunning()
    {
        Console.WriteLine("[RuntimeExample Server] Hello World! from a compiled DLL");
        On.Terraria.Program.LaunchGame += Program_LaunchGame;
        On.Terraria.NetMessage.greetPlayer += NetMessage_greetPlayer;
    }

    private static void NetMessage_greetPlayer(On.Terraria.NetMessage.orig_greetPlayer orig, int plr)
    {
        Console.WriteLine($"[RuntimeExample Server] Greet player: {plr}");
        orig(plr);
    }

    private static void Program_LaunchGame(On.Terraria.Program.orig_LaunchGame orig, string[] args, bool monoArgs)
    {
        Console.WriteLine("[RuntimeExample Server] LaunchGame was called!");
        orig(args, monoArgs);
    }
}
