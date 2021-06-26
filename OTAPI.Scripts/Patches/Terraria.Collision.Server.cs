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

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

/// <summary>
/// @doc Fixes "Index out of range" exceptions wihin Terraria.Collision.GetEntityEdgeTiles
/// </summary>
namespace Terraria
{
    partial class patch_Collision : Terraria.Collision
    {
        // fix index out of range exceptions in this method.
        public static extern List<Point> orig_GetEntityEdgeTiles(Entity entity, bool left = true, bool right = true, bool up = true, bool down = true);
        public static List<Point> GetEntityEdgeTiles(Entity entity, bool left = true, bool right = true, bool up = true, bool down = true)
        {
            var result = orig_GetEntityEdgeTiles(entity, left, right, up, down);

            for (var i = 0; i < result.Count; i++)
            {
                var pnt = result[i];

                pnt.X = Math.Max(0, result[i].X);
                pnt.X = Math.Min(Main.maxTilesX, pnt.X);

                pnt.Y = Math.Max(0, pnt.Y);
                pnt.Y = Math.Min(Main.maxTilesY, pnt.Y);

                result[i] = pnt;
            }

            return result;
        }

    }
}