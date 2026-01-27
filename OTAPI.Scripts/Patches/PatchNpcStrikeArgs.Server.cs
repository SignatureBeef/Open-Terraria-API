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
#if !tModLoaderServer_V1_3
using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;
using System;
using System.Linq;

/// <summary>
/// @doc Adds a Terraria.Entity entity parameter to Terraria.NPC.StrikeNPC.
/// </summary>
[MonoModIgnore]
partial class NpcStrikeArgs
{
    [Modification(ModType.PreMerge, "Patching in entity source for NPC strike")]
    static void PatchNpcStrikeArgs(ModFwModder modder)
    {
#if TerrariaServer_1450_OrAbove || Terraria__1450_OrAbove || tModLoader_1450_OrAbove
        var csr = modder.GetILCursor(() => (new Terraria.NPC()).StrikeNPC(0, 0, 0, false, false, false, 0));
#else
        var csr = modder.GetILCursor(() => (new Terraria.NPC()).StrikeNPC(0, 0, 0, false, false, false));
#endif
        
        var redirects = csr.Method.DeclaringType.Methods
            .Where(x => (HookEmitter.HookMethodNamePrefix + x.Name) == csr.Method.Name || ("orig_" + x.Name) == csr.Method.Name)
            .Select(x => x.GetILCursor())
            .ToArray();

        foreach (var method in redirects.Append(csr))
        {
            ParameterDefinition Entity;
            method.Method.Parameters.Add(Entity = new ("entity",
                ParameterAttributes.HasDefault | ParameterAttributes.Optional,

                modder.Module.ImportReference(modder.GetDefinition<Terraria.Entity>())
            )
            {
                Constant = null
            });

            modder.OnRewritingMethodBody += (MonoModder modder, MethodBody body, Instruction instr, int instri) =>
            {
                if (instr.Operand is MethodReference methodReference)
                {
                    if (methodReference.DeclaringType.Name == method.Method.DeclaringType.Name
                        && methodReference.Name == method.Method.Name)
                    {
                        if (methodReference.Parameters.Any(x => x.Name == Entity.Name))
                        {
                            return;
                        }
                        methodReference.Parameters.Add(Entity);

                        var methodName = body.Method.DeclaringType.Name + "." + body.Method.Name;
                        switch (methodName.Replace(HookEmitter.HookMethodNamePrefix, ""))
                        {
                            case "MessageBuffer.GetData":
                                body.GetILProcessor().InsertBefore(instr,
                                    new { OpCodes.Ldsfld, Operand = modder.Module.ImportReference(modder.GetFieldDefinition(() => Terraria.Main.player)) },
                                    new { OpCodes.Ldarg_0 },
                                    new { OpCodes.Ldfld, Operand = modder.Module.ImportReference(modder.GetFieldDefinition(() => (new Terraria.MessageBuffer()).whoAmI)) },
                                    new { OpCodes.Ldelem_Ref }
                                );
                                break;

                            case "NPC.StrikeNPCNoInteraction":
                                // this is via world, so null is expected.
                                body.GetILProcessor().InsertBefore(instr,
                                    new { OpCodes.Ldnull }
                                );
                                break;

                            case "Player.ApplyDamageToNPC":
                            case "Player.ItemCheck_MeleeHitNPCs":
                            case "Projectile.Damage":
                            case "Player.ProcessHitAgainstNPC":
                            case "NPC.StrikeNPC":
                                body.GetILProcessor().InsertBefore(instr,
                                    new { OpCodes.Ldarg_0 }
                                );
                                break;

                            case "Projectile.Damage_PVE_Inner":
                                // find the NPC parameter
                                var prm = body.Method.Parameters.Single(x => x.ParameterType.FullName == "Terraria.NPC");
                                body.GetILProcessor().InsertBefore(instr,
                                    new { OpCodes.Ldarg, Operand = prm }
                                );
                                break;

                            default:
                                throw new NotImplementedException($"{body.Method.FullName} is not a supported caller for this modification");
                        }
                    }
                }
            };
        }
    }
}
#endif