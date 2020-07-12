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

using OTAPI;

namespace Terraria
{
    class patch_Projectile : Terraria.Projectile
    {
        /** Begin Hook - SetDefaults */
        public extern void orig_SetDefaults(int Type);
        public void SetDefaults(int Type)
        {
            if (Hooks.Projectile.SetDefaults?.Invoke(HookEvent.Before, this, ref Type, orig_SetDefaults) != HookResult.Cancel)
            {
                orig_SetDefaults(Type);
                Hooks.Projectile.SetDefaults?.Invoke(HookEvent.After, this, ref Type, orig_SetDefaults);
            }
        }
        /** End Hook - SetDefaults */

        /** Begin Hook - Update */
        public extern void orig_Update(int i);
        public void Update(int i)
        {
            if (Hooks.Projectile.Update?.Invoke(HookEvent.Before, this, ref i, orig_Update) != HookResult.Cancel)
            {
                orig_Update(i);
                Hooks.Projectile.Update?.Invoke(HookEvent.After, this, ref i, orig_Update);
            }
        }
        /** End Hook - Update */
    }
}

namespace OTAPI
{
    public static partial class Hooks
    {
        public static class Projectile
        {
            public delegate HookResult SetDefaultsHandler(HookEvent @event, Terraria.Projectile instance, ref int type, Action<int> originalMethod);
            public static SetDefaultsHandler SetDefaults;

            public delegate HookResult UpdateHandler(HookEvent @event, Terraria.Projectile instance, ref int i, Action<int> originalMethod);
            public static UpdateHandler Update;
        }
    }
}
