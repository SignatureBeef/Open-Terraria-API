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
#pragma warning disable CS0436 // Type conflicts with imported type

using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;

/// <summary>
/// @doc Creates Terraria.NPC.OnSetDefaultType. Allows plugins to hook when NPC instances need to set custom type values in SetDefaults.
/// </summary>
[Modification(ModType.PreMerge, "Adding Terraria.NPC.OnSetDefaultType()")]
[MonoMod.MonoModIgnore]
void CreateOnSetDefaultType(MonoModder modder)
{
#if tModLoaderServer_V1_3
    var SetDefaults = modder.GetILCursor(() => new Terraria.NPC().SetDefaults(0, 0f));
#else
    var SetDefaults = modder.GetILCursor(() => new Terraria.NPC().SetDefaults(0, default(Terraria.NPCSpawnParams)));
#endif

    SetDefaults.GotoNext(
        i => i.Operand is FieldReference fieldReference && fieldReference.Name == "dedServ" && fieldReference.DeclaringType.FullName == "Terraria.Main"
    );

    var callback = new MethodDefinition("OnSetDefaultType", MethodAttributes.Public, SetDefaults.Module.TypeSystem.Void);
    SetDefaults.Method.DeclaringType.Methods.Add(callback);
    callback.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

    SetDefaults.Emit(OpCodes.Ldarg_0);
    SetDefaults.Emit(OpCodes.Call, callback);

    SetDefaults.Next.ReplaceTransfer(SetDefaults.Prev.Previous, SetDefaults.Method);
}