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
using ModFramework;
using System;

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

        /** Begin Hook - Update */
        protected extern void orig_Update(GameTime gameTime);
        protected void Update(GameTime gameTime)
        {
            if (Hooks.Main.Update?.Invoke(HookEvent.Before, ref gameTime) != HookResult.Cancel)
            {
                orig_Update(gameTime);
                Hooks.Main.Update?.Invoke(HookEvent.After, ref gameTime);
            }
        }
        /** End Hook - Update */

        /** Begin Hook - Initialize */
        protected extern void orig_Initialize();
        protected void Initialize()
        {
            if (Hooks.Main.Initialize?.Invoke(HookEvent.Before) != HookResult.Cancel)
            {
                orig_Initialize();
                Hooks.Main.Initialize?.Invoke(HookEvent.After);
            }
        }
        /** End Hook - Initialize */

        /** Begin Hook - startDedInput */
        public extern static void orig_startDedInput();
        public static void startDedInput()
        {
            if (Hooks.Main.startDedInput?.Invoke(HookEvent.Before, orig_startDedInput) != HookResult.Cancel)
            {
                orig_startDedInput();
                Hooks.Main.startDedInput?.Invoke(HookEvent.After, orig_startDedInput);
            }
        }
        /** End Hook - startDedInput */
    }
}

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Main
        {
            public delegate HookResult UpdateHandler(HookEvent @event, ref GameTime gameTime);
            public static UpdateHandler Update;

            public delegate HookResult InitializeHandler(HookEvent @event);
            public static InitializeHandler Initialize;

            public delegate HookResult startDedInputHandler(HookEvent @event, Action originalMethod);
            public static startDedInputHandler startDedInput;
        }
    }
}
