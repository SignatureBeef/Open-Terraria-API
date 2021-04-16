using System;
using System.Collections.Generic;
using System.Linq;
using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;

[MonoModIgnore]
partial class CollisionSwitchTiles
{
    static ParameterDefinition Entity { get; set; }
    static MethodDefinition SwitchTiles { get; set; }

    [Modification(ModType.PreMerge, "Patching Collision.SwitchTiles")]
    static void ModifyCollisionSwitchTiles(ModFramework.ModFwModder modder)
    {
        var csr = modder.GetILCursor(() => Terraria.Collision.SwitchTiles(default, 0, 0, default, 0), followRedirect: true);
        SwitchTiles = csr.Method;

        csr.Method.Parameters.Add(Entity = new ParameterDefinition("entity",
            ParameterAttributes.HasDefault | ParameterAttributes.Optional,

            modder.Module.ImportReference(modder.GetDefinition<Terraria.Entity>())
        )
        {
            Constant = null
        });

        modder.OnRewritingMethodBody += PatchCollisionSwitchTiles_OnRewritingMethodBody;

        // inject the callback
        {
            // find all the HitSwitch calls
            // add a branch around them, until after the SendData call

            var calls = csr.Body.Instructions.Where(ins => ins.Operand is MethodReference mref && mref.Name == "HitSwitch").ToArray();

            foreach (var hitswitch in calls)
            {
                var arg_x = hitswitch.Previous.Previous;
                var arg_y = hitswitch.Previous;

                csr.Goto(hitswitch, MonoMod.Cil.MoveType.Before);

                //// find the continuation branch (via SendData)
                //var continuation = hitswitch.Next(ins => ins.Operand is MethodReference mref && mref.Name == "SendData");

                csr.Emit(OpCodes.Ldarg, Entity);
                csr.EmitDelegate<PressurePlateCallback>(OTAPI.Callbacks.Collision.PressurePlate);
                var cancellation = csr.EmitAll(
                    new { OpCodes.Nop },
                    new { OpCodes.Ldc_I4_0 },
                    new { OpCodes.Ret }
                );

                // we consumed the stack with our callback, readd the x/y for the HitSwitch call to use
                var continuation = csr.EmitAll(
                    new { arg_x.OpCode, arg_x.Operand },
                    new { arg_y.OpCode, arg_y.Operand }
                );

                var nop = cancellation.First();
                nop.OpCode = OpCodes.Brtrue_S;
                nop.Operand = continuation.First();
            }
        }
    }

    private static void PatchCollisionSwitchTiles_OnRewritingMethodBody(MonoModder modder, MethodBody body, Instruction instr, int instri)
    {
        if (instr.Operand is MethodReference methodReference)
        {
            if (methodReference.DeclaringType.Name == SwitchTiles.DeclaringType.Name
                && methodReference.Name == SwitchTiles.Name)
            {
                if (methodReference.Parameters.Any(x => x.Name == Entity.Name))
                {
                    return;
                }
                methodReference.Parameters.Add(Entity);

                if (body.Method.DeclaringType.BaseType.FullName == typeof(Terraria.Entity).FullName)
                {
                    body.GetILProcessor().InsertBefore(instr,
                        new { OpCodes.Ldarg_0 }
                    );
                }
                else throw new NotImplementedException($"{body.Method.Name} is not a supported caller for this modification");
            }
        }
    }
}


[MonoMod.MonoModIgnore]
public delegate bool PressurePlateCallback(int x, int y, Terraria.Entity entity);

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Collision
        {
            public delegate HookResult PressurePlateHandler(int x, int y, Terraria.Entity entity);
            public static PressurePlateHandler PressurePlate;
        }
    }
}

namespace OTAPI.Callbacks
{
    public static partial class Collision
    {
        public static bool PressurePlate(int x, int y, Terraria.Entity entity)
        {
            return Hooks.Collision.PressurePlate?.Invoke(x, y, entity) != HookResult.Cancel;
        }
    }
}
