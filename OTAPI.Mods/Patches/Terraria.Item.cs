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
    class patch_Item: Terraria.Item
    {
        /** Begin Hook - SetDefaults */
        public extern void orig_SetDefaults(int Type, bool noMatCheck = false);
        public void SetDefaults(int Type, bool noMatCheck = false)
        {
            if (Hooks.Item.SetDefaults?.Invoke(HookEvent.Before, this, ref Type, ref noMatCheck, orig_SetDefaults) != HookResult.Cancel)
            {
                orig_SetDefaults(Type);
                Hooks.Item.SetDefaults?.Invoke(HookEvent.After, this, ref Type, ref noMatCheck, orig_SetDefaults);
            }
        }
        /** End Hook - SetDefaults */

        /** Begin Hook - Update */
        public extern void orig_UpdateItem(int i);
        public void UpdateItem(int i)
        {
            if (Hooks.Item.UpdateItem?.Invoke(HookEvent.Before, this, ref i, orig_UpdateItem) != HookResult.Cancel)
            {
                orig_UpdateItem(i);
                Hooks.Item.UpdateItem?.Invoke(HookEvent.After, this, ref i, orig_UpdateItem);
            }
        }
        /** End Hook - Update */
    }
}
