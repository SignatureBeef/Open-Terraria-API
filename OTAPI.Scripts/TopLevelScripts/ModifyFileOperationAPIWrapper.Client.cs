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
using ModFramework;
using MonoMod;

/// <summary>
/// @doc Patches FileOperationAPIWrapper.SHFILEOPSTRUCT to work for x64
/// </summary>
[Modification(ModType.PreMerge, "Patching FileOperationAPIWrapper.SHFILEOPSTRUCT to work for x64")]
void ModifyFileOperationAPIWrapper(ModFwModder modder)
{
    // windows only mod, but tls needs a Main()
    var type = modder.Module.GetType("Terraria.Utilities.FileOperationAPIWrapper/SHFILEOPSTRUCT");
    if (type is not null) type.PackingSize = 0;
}