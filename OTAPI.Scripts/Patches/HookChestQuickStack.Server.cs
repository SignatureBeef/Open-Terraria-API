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
#pragma warning disable CS0436 // Type conflicts with imported type

using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;
using MonoMod.Cil;
using System;
using System.Linq;

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Chest
        {
            public class QuickStackEventArgs : EventArgs
            {
                public HookResult? Result { get; set; }

                public int PlayerId { get; set; }
                public Terraria.Item Item { get; set; }
                public int ChestIndex { get; set; }
            }
            public static event EventHandler<QuickStackEventArgs> QuickStack;

            public static bool InvokeQuickStack(int playerId, Terraria.Item item, int chestIndex)
            {
                var args = new QuickStackEventArgs()
                {
                    PlayerId = playerId,
                    Item = item,
                    ChestIndex = chestIndex,
                };
                QuickStack?.Invoke(null, args);
                return args.Result != HookResult.Cancel;
            }
        }
    }
}