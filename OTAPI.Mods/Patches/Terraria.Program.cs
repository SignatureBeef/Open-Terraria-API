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
using MonoMod;
using OTAPI;
using System;

namespace Terraria
{
    class patch_Program
    {
        /** Begin Hook: LaunchGame */
        public static extern void orig_LaunchGame(string[] args, bool monoArgs = false);
        public static void LaunchGame(string[] args, bool monoArgs = false)
        {
            if (Hooks.Program.LaunchGame == null || Hooks.Program.LaunchGame() == HookResult.Continue)
            {
                orig_LaunchGame(args, monoArgs);
            }
        }
        /** End Hook: Launch */

        /** Begin Cross platform support - Avoid System.Windows.Forms */
        public static extern void orig_DisplayException(Exception e);
        private static void DisplayException(Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        /** End Cross platform support - Avoid System.Windows.Forms */
    }
}
