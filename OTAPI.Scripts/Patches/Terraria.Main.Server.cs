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

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it

namespace Terraria
{
    class patch_Main : Terraria.Main
    {
        public extern void orig_NeverSleep();
        public void NeverSleep()
        {
            if (ReLogic.OS.Platform.IsWindows) orig_NeverSleep();
        }

        public extern void orig_YouCanSleepNow();
        public void YouCanSleepNow()
        {
            if (ReLogic.OS.Platform.IsWindows) orig_YouCanSleepNow();
        }

        // this was implemented in v2. by looking at the vanilla code it will auto
        // save regardless of this flag, so we enforce the check here in case
        // its disabled for a reason.
        public static extern void orig_DoUpdate_AutoSave();
        public static void DoUpdate_AutoSave()
        {
            if(autoSave)
            {
                orig_DoUpdate_AutoSave();
            }
        }
    }
}