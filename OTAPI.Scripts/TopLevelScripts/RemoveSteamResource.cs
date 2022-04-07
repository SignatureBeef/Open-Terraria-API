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

using ModFramework;
using MonoMod;
using System.Linq;

/// <summary>
/// @doc Removes steam from embedded resources as its replaced
/// </summary>
[Modification(ModType.PreMerge, "Removing Steam Embedded Resource")]
[MonoMod.MonoModIgnore]
void RemoveSteamResource(MonoModder modder)
{
	var sw = modder.Module.Resources.Single(r => r.Name.EndsWith("Steamworks.NET.dll", System.StringComparison.CurrentCultureIgnoreCase));
	modder.Module.Resources.Remove(sw);
}