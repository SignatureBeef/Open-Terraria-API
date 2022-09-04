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
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;
using MonoMod.Cil;

/// <summary>
/// @doc Attaches newtwork packets to the Terraria.Main.ServerSideCharacter variable. This allows sub projects like TShock to activate SSC. 
/// </summary>
[Modification(ModType.PreMerge, "Enabling server side characters")]
[MonoMod.MonoModIgnore]
void EnableServerSideCharacters(MonoModder modder)
{
    // find the occurance of downedClown and insert the SSC field to the missing BitsByte instance
#if TerrariaServer_SendDataNumber8
    var sendData = modder.GetILCursor(() => Terraria.NetMessage.SendData(default, default, default, default, default, default, default, default, default, default, default, default));
#else
    var sendData = modder.GetILCursor(() => Terraria.NetMessage.SendData(default, default, default, default, default, default, default, default, default, default, default));
#endif
    var sscField = modder.GetFieldDefinition(() => Terraria.Main.ServerSideCharacter);

    sendData.GotoNext(MoveType.After, (ins) => ins.MatchLdsfld("Terraria.NPC", "downedClown"));
    sendData.GotoNext(MoveType.After, (ins) => ins.OpCode == OpCodes.Call);

    var bitsByteInstance = sendData.Next.Operand as VariableDefinition;  // Terraria.BitsByte instance
    var setItem = sendData.Previous.Operand as MethodReference;  // Terraria.BitsByte::set_Item

    sendData.Emit(OpCodes.Ldloca_S, bitsByteInstance);
    sendData.Emit(OpCodes.Ldc_I4_6);
    sendData.Emit(OpCodes.Ldsfld, sscField);
    sendData.Emit(OpCodes.Call, setItem);
}
