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
using Microsoft.Xna.Framework;
using OTAPI;

namespace Terraria
{
    class patch_Main
    {
        /** Begin Cross platform support - Avoid Windows specific calls */
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
        /** End Cross platform support - Avoid Windows specific calls */

        /** Begin Hook - Pre/PostUpdate */
        protected extern void orig_Update(GameTime gameTime);
        protected void Update(GameTime gameTime)
        {
            if (Hooks.Main.Update?.Invoke(HookEvent.Pre, ref gameTime) == HookResult.Continue)
            {
                orig_Update(gameTime);
                Hooks.Main.Update?.Invoke(HookEvent.Post, ref gameTime);
            }
        }
        /** End Hook - Pre/PostUpdate */

        /** Begin Hook - Pre/PostInitialize */
        protected extern void orig_Initialize();
        protected void Initialize()
        {
            if (Hooks.Main.Initialize?.Invoke(HookEvent.Pre) == HookResult.Continue)
            {
                orig_Initialize();
                Hooks.Main.Initialize?.Invoke(HookEvent.Post);
            }
        }
        /** End Hook - Pre/PostInitialize */
    }
}
