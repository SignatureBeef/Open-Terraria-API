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
namespace OTAPI.Mods
{
    public interface IMod
    {
        /// <summary>
        /// Name of the mod
        /// </summary>
        Terraria.Localization.LocalizedText? Name { get; }

        /// <summary>
        /// Type ID used for Terraria, e.g. NPC.NewNPC
        /// </summary>
        int TypeID { get; }

        /// <summary>
        /// Fired when the entity is registered with OTAPI, not when instanced.
        /// </summary>
        void Registered();
    }
}
