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
System.Console.WriteLine("Steamworks.NET patch not available in TML1.4");
#else
using ModFramework;
using Mono.Cecil;
using MonoMod;
using System.IO;
using System.Linq;

/// <summary>
/// @doc Patches to the current steamworks.net binary
/// </summary>
[Modification(ModType.PreMerge, "Patching Steamworks.NET")]
[MonoMod.MonoModIgnore]
void PatchSteam(MonoModder modder)
{
    var desired = typeof(Steamworks.SteamShutdown_t).Assembly.GetName().Version;

    //Update the references to match what is installed
    foreach (var reference in modder.Module.AssemblyReferences)
    {
        if (reference.Name == "Steamworks.NET")
        {
            reference.Version = desired;
            break;
        }
    }

    //Remove the embedded Newtonsoft resource
    modder.Module.Resources.Remove(
        modder.Module.Resources.Single(x => x.Name.EndsWith("Steamworks.NET.dll"))
    );
}
#endif