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
using System;
using System.IO;
using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;

/// <summary>
/// @doc A mod that changes the base path of native assembly loading
/// </summary>
[Modification(ModType.PostPatch, "Updating native assembly loading")]
[MonoMod.MonoModIgnore]
void PathMonoLaunch(MonoModder modder)
{
    var rnl = modder.GetILCursor(() => MonoLaunch.ResolveNativeLibrary(null, null));
    var gbd = modder.GetMethodDefinition(() => patch_MonoLaunch.GetBaseDirectory(), followRedirect: true);

    rnl.GotoNext(i => i.OpCode == OpCodes.Call && i.Operand is MethodReference mref && mref.Name == "get_CurrentDirectory");

    rnl.Next.Operand = gbd;
}

partial class patch_MonoLaunch
{
    public static string GetBaseDirectory()
    {
        return Path.Combine(Environment.CurrentDirectory, "tModLoader");
    }
}
#endif