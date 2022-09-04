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

#if !tModLoader_V1_4
System.Console.WriteLine("PatchPrivateClasses only needed for TML");
#else
using ModFramework;
using MonoMod;

/// <summary>
/// @doc Patches Terraria.Program to be pubic so that JIT methods can be accessed
/// </summary>
[Modification(ModType.PostPatch, "Patching various private methods for public access")]
[MonoMod.MonoModIgnore]
void PatchPrivateClasses(MonoModder modder)
{
    modder.Module.GetType("MonoLaunch").SetPublic(true);
    modder.Module.GetType("Terraria.Program").SetPublic(true);
    var iv = modder.Module.GetType("Terraria.ModLoader.Engine.InstallVerifier");
    iv.SetPublic(true);
    iv.Field("steamAPIPath").SetPublic(true);
}
#endif
