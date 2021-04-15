using System;
using System.Collections.Generic;
using System.Linq;
using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;

partial class Development
{
    static ParameterDefinition Entity { get; set; }
    static MethodDefinition StrikeNPC { get; set; }

    [Modification(ModType.PreMerge, "Patching in entity source for NPC strike")]
    static void PatchNpcStrikeArgs(ModFramework.ModFwModder modder)
    {
        var csr = modder.GetILCursor(() => (new Terraria.NPC()).StrikeNPC(0, 0, 0, false, false, false), followRedirect: true);
        StrikeNPC = csr.Method;

        csr.Method.Parameters.Add(Entity = new ParameterDefinition("entity",
            ParameterAttributes.HasDefault | ParameterAttributes.Optional,

            modder.Module.ImportReference(modder.GetDefinition<Terraria.Entity>())
        )
        {
            Constant = null
        });

        modder.OnRewritingMethodBody += Modder_OnRewritingMethodBody;
    }

    private static void Modder_OnRewritingMethodBody(MonoModder modder, MethodBody body, Instruction instr, int instri)
    {
        if (instr.Operand is MethodReference methodReference)
        {
            if (methodReference.DeclaringType.Name == StrikeNPC.DeclaringType.Name
                && methodReference.Name == StrikeNPC.Name)
            {
                if (methodReference.Parameters.Any(x => x.Name == Entity.Name))
                {
                    return;
                }
                methodReference.Parameters.Add(Entity);

                switch (body.Method.DeclaringType.Name + "." + body.Method.Name)
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
                        body.GetILProcessor().InsertBefore(instr,
                            new { OpCodes.Ldarg_0 }
                        );
                        break;

                    default:
                        throw new NotImplementedException($"{body.Method.Name} is not a supported caller for this modification");
                }
            }
        }
    }
}
