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

#if tModLoaderServer_V1_3
System.Console.WriteLine("RemoveMap not available in TML1.3. Might be future support, depends if mods need to extend this for client related mods.");
#else
using ModFramework;
using MonoMod;

/// <summary>
/// @doc Disables the world map to be created on the server (it is not needed, at least on vanilla).
/// </summary>
[Modification(ModType.PreMerge, "Removing world map")]
[MonoMod.MonoModIgnore]
void RemoveMap(MonoModder modder)
{
	var worldMap = modder.GetDefinition<Terraria.Map.WorldMap>();
	foreach (var method in worldMap.Methods)
	{
		method.ClearBody();
	}
}
#endif