using Mono.Cecil;
using Mono.Cecil.Cil;
using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modifications.Helpers;
using System;
using System.Linq;

namespace OTAPI.Patcher.Modifications.Hooks.Npc
{
    public class Transform : OTAPIModification<OTAPIContext>
    {
        public override void Run(OptionSet options)
        {
            Console.Write("Hooking Npc.Transform...");

            var vanilla = this.Context.Terraria.Types.Npc.Method("Transform");
            var callback = this.Context.OTAPI.Types.Npc.Method("Transform");

            //We could wrap this method, but this hooks is to inform when an npc transforms, not
            //and occurance of the call.
            //However, in the future we might actually provide the begin/end calls for a seperate hook.
            //Anyway, this instance we need to insert before the netMode == 2 check.

            var insertionPoint = vanilla.Body.Instructions.Single(x => x.OpCode == OpCodes.Ldsfld
               && x.Operand is FieldReference
               && (x.Operand as FieldReference).Name == "netMode"
               && x.Next.OpCode == OpCodes.Ldc_I4_2
            );

            var il = vanilla.Body.GetILProcessor();

            //Insert our callback before the if block, ensuring we consider that the if block may be referenced elsewhere
            Instruction ourEntry;
            il.InsertBefore(insertionPoint, ourEntry=il.Create(OpCodes.Ldarg_0)); //Add the current instance (this in C#) to the callback
            il.InsertBefore(insertionPoint, il.Create(OpCodes.Call, vanilla.Module.Import(callback)));

            //Replace transfers
            insertionPoint.ReplaceTransfer(ourEntry, vanilla);

            Console.WriteLine("Done");
        }
    }
}
