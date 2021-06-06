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
        var csr = modder.GetILCursor(() => Terraria.Collision.SwitchTiles(default, 0, 0, default, 0));
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
            public class PressurePlateEventArgs : EventArgs
            {
                public HookResult? Result { get; set; }

                public int x { get; set; }
                public int y { get; set; }
                public Terraria.Entity entity { get; set; }
            }
            public static event EventHandler<PressurePlateEventArgs> PressurePlate;

            public static HookResult? InvokePressurePlate(PressurePlateEventArgs args)
            {
                PressurePlate?.Invoke(null, args);
                return args.Result;
            }
        }
    }
}

namespace OTAPI.Callbacks
{
    public static partial class Collision
    {
        public static bool PressurePlate(int x, int y, Terraria.Entity entity)
        {
            var args = new Hooks.Collision.PressurePlateEventArgs()
            {
                x = x,
                y = y,
                entity = entity
            };
            return Hooks.Collision.InvokePressurePlate(args) != HookResult.Cancel;
        }
    }
}
