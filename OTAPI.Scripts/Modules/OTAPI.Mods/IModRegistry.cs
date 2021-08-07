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
using System.Collections.Generic;

namespace OTAPI.Mods
{
    internal static class IModRegistry
    {
        private static Dictionary<Type, int> _types = new Dictionary<Type, int>();

        /// <summary>
        /// Allocates a type value for a ModType, usually extending from an existing vanilla offset
        /// </summary>
        /// <typeparam name="TMod">The mod that needs to be assigned a Type</typeparam>
        /// <param name="vanillaOffset">The offset for if vanilla already has a base collection</param>
        /// <returns>The value to be used</returns>
        public static int AllocateType<TMod>(int vanillaOffset)
        {
            if (!_types.TryGetValue(typeof(TMod), out var mod))
                mod = vanillaOffset;

            mod++;

            _types[typeof(TMod)] = mod;

            return mod;
        }
    }
}
