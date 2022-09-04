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
System.Console.WriteLine("MonoMod patch only needed for TML");
#else
using ModFramework;
using MonoMod;

/// <summary>
/// @doc A mod to update MonoMod assembly references.
/// </summary>
[Modification(ModType.PostPatch, "Updating MonoMod libs")]
[MonoMod.MonoModIgnore]
void PatchMonoMod(MonoModder modder)
{
    var desired = typeof(MonoMod.MonoModder).Assembly.GetName().Version;

    //Update the references to match what is installed
    foreach (var reference in modder.Module.AssemblyReferences)
    {
        if (reference.Name.Contains("MonoMod"))
        {
            reference.Version = desired;
        }
    }
}
#endif