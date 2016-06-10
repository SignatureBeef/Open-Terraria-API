using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modifications.Helpers;
using System;
using System.Linq;

namespace OTAPI.Patcher.Modifications.Hooks.World
{
    /// <summary>
    /// In this modification we will insert our callback to be the return value 
    /// of the MechSpawn function in both NPC and Item. 
    /// Additionally we also append all the arguments and some local variables so we have 
    /// more data to work with.
    /// </summary>
    public class Statue : OTAPIModification<OTAPIContext>
    {
        public override void Run(OptionSet options)
        {
            Console.Write("Hooking statue spawning...");

            foreach (var type in new[]
            {
                new { TypeDef = this.Context.Terraria.Types.Npc, MechType = OpCodes.Ldc_I4_1 },
                new { TypeDef = this.Context.Terraria.Types.Item, MechType = OpCodes.Ldc_I4_2}
            })
            {
                var vanilla = type.TypeDef.Methods.Single(x => x.Name == "MechSpawn");
                var hook = this.Context.Terraria.MainModue.Import(this.Context.OTAPI.Types.World.Method("MechSpawn"));

                //Here we find the insertion point where we want to inject our callback at.
                var iInsertionPoint = vanilla.Body.Instructions.Last(x => x.OpCode == OpCodes.Ldloc_1);
                //If the result of the callback instructs us to cancel, this instruction will transfer us to the existing "return false"
                var iContinuePoint = vanilla.Body.Instructions.Last(x => x.OpCode == OpCodes.Ldc_I4_0);

                var il = vanilla.Body.GetILProcessor();

                //Add all the parameters
                foreach (var prm in vanilla.Parameters)
                    il.InsertBefore(iInsertionPoint, il.Create(OpCodes.Ldarg, prm));

                //Add the first three variables by reference
                for (var x = 0; x < 3; x++)
                    il.InsertBefore(iInsertionPoint, il.Create(OpCodes.Ldloca, vanilla.Body.Variables[x]));

                //Append our mech type enum value to the method call
                il.InsertBefore(iInsertionPoint, il.Create(type.MechType));

                //Trigger the method to run with the parameters on the stck
                il.InsertBefore(iInsertionPoint, il.Create(OpCodes.Call, hook));

                //If the result left on the stack is false, then jump to the "return false"
                il.InsertBefore(iInsertionPoint, il.Create(OpCodes.Brfalse_S, iContinuePoint));

                vanilla.Body.OptimizeMacros();
            }

            Console.WriteLine("Done");
        }
    }
}
