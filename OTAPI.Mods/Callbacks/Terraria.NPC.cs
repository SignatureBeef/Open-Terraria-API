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

namespace OTAPI.Callbacks
{
    public static class NPC
    {
        public static void Killed(Terraria.NPC instance)
            => Hooks.NPC.Killed?.Invoke(instance);
        public static bool Spawn(ref int index)
            => Hooks.NPC.Spawn?.Invoke(ref index) != HookResult.Cancel;

#if TerrariaServer_V1_4
        public static int DropLoot(
                int X, int Y, int Width, int Height, int Type,
                int Stack, bool noBroadcast, int pfix, bool noGrabDelay, bool reverseLookup,
                Terraria.NPC instance)
        {
            int itemIndex = 0;
            if (Hooks.NPC.DropLoot?.Invoke(HookEvent.Before, instance, ref itemIndex, ref X, ref Y, ref Width, ref Height, ref Type, ref Stack, ref noBroadcast, ref pfix, ref noGrabDelay, ref reverseLookup) != HookResult.Cancel)
            {
                itemIndex = Terraria.Item.NewItem(X, Y, Width, Height, Type, Stack, noBroadcast, pfix, noGrabDelay, reverseLookup);
                Hooks.NPC.DropLoot?.Invoke(HookEvent.After, instance, ref itemIndex, ref X, ref Y, ref Width, ref Height, ref Type, ref Stack, ref noBroadcast, ref pfix, ref noGrabDelay, ref reverseLookup);
            }
            return itemIndex;
        }
#endif
    }
}
