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
#pragma warning disable CS8321 // Local function is declared but never used

#if tModLoader_V1_4
System.Console.WriteLine("ITile not available in TML");
#else
using ModFramework;
using ModFramework.Relinker;

/// <summary>
/// @doc Transforms the fields of Terraria.Tile into properties
/// </summary>
[Modification(ModType.PreMerge, "Implementing Tile properties")]
[MonoMod.MonoModIgnore]
void ITileProperties(ModFwModder modder)
{
    var tile = modder.GetDefinition<Terraria.Tile>();
    tile.RemapFieldsToProperties(modder);
}
#endif