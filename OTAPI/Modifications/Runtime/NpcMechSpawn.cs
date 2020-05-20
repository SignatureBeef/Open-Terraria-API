using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Linq;

namespace OTAPI.Modifications.Runtime
{
    [Modification("Modifying Terraria.NPC.MechSpawn")]
    class NpcMechSpawn
    {
        public NpcMechSpawn()
        {
            IL.Terraria.NPC.MechSpawn += il => Modify(il, (bool returnValue, float x, float y, int type, int num, int num2, int num3, int i, Microsoft.Xna.Framework.Vector2 vector, float num6) =>
            {
                //Console.WriteLine($"Terraria.NPC.MechSpawn returnValue={returnValue}, x={x}, y={y}, type={type}, num={num}, num2={num2}, num3={num3}, i={i}, vector={vector.X},{vector.Y}, num6={num6}");
                if (Hooks.NPC.MechSpawn == null || Hooks.NPC.MechSpawn(x, y, type, num, num2, num3, i, vector, num6) == HookResult.Continue)
                {
                    return returnValue;
                }
                return false;
            });
        }

        protected void Modify(ILContext il, Func<bool, float, float, int, int, int, int, int, Microsoft.Xna.Framework.Vector2, float, bool> callback)
        {
            // capture each return in the method body, then add all of the parameters and local variables to the stack, then call our callback.
            // this would then be, callback(true/false, x, y, type. num, num2, num3);

            //ILCursor c = new ILCursor(il);
            //var returns = il.Body.Instructions.Where(x => x.OpCode == OpCodes.Ret).ToArray();

            //foreach (var ret in returns)
            //{
            //    c.Goto(ret);
            //    foreach (var parameter in il.Method.Parameters)
            //    {
            //        //Console.WriteLine($"Added param:{parameter.ParameterType.FullName}");
            //        c.Emit(OpCodes.Ldarg, parameter);
            //    }
            //    foreach (var variable in il.Body.Variables)
            //    {
            //        //Console.WriteLine($"Added var: {variable.VariableType.FullName}");
            //        c.Emit(OpCodes.Ldloc, variable);
            //    }
            //    c.EmitDelegate<Func<bool, float, float, int, int, int, int, int, Microsoft.Xna.Framework.Vector2, float, bool>>(callback);
            //}
        }
    }
}
