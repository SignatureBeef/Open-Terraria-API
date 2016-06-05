using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modifications.Helpers;
using System;
using System.Linq;

namespace OTAPI.Patcher.Modifications.Hooks.Npc
{
    public class DropLoot_1_NetDrop : OTAPIModification<OTAPIContext>
    {
        public override void Run(OptionSet options)
        {
            Console.Write("Hooking Npc.NPCLoot\\SendData...");

            var npcLoot = this.Context.Terraria.Types.Npc.Method("NPCLoot");
            var dropLoot = this.Context.Terraria.Types.Npc.Method("DropLoot");

            //In the NPCLoot method there is a call to send packet 88 (after item drop).
            //We will also want to hook this in the case the returned value from DropLoot
            //cancels the event.
            //Note, each update will need checking for any other calls to DropLoot that populate variables
            //as currently this is the only one.
            //TODO: write a test for this

            var il = npcLoot.Body.GetILProcessor();

            //This section will add '&& num40 >= 0' to the statement above "Main.item [num40].color = this.color;"
            var insColour = npcLoot.Body.Instructions.Single(x => x.OpCode == OpCodes.Ldfld && x.Operand == this.Context.Terraria.Types.Npc.Field("color")); //Grab where the call is located
            var insColorStart = insColour.Previous(i => i.OpCode == OpCodes.Ldsfld); //Find the first instruction for the color call
            var resumeInstruction = insColorStart.Previous.Operand as Instruction; //Find the instruction where it should be transferred to if false is evaludated

            il = npcLoot.Body.GetILProcessor();

            //Insert the num40 variable (the result back from the DropLoot method)
            il.InsertBefore(insColorStart, il.Create(OpCodes.Ldloc, (VariableDefinition)insColorStart.Next.Operand));
            //Place 0 on the stack
            il.InsertBefore(insColorStart, il.Create(OpCodes.Ldc_I4_0));
            //Compare the current values on stack, using >=
            il.InsertBefore(insColorStart, il.Create(OpCodes.Blt, resumeInstruction));

            npcLoot.Body.OptimizeMacros();

            Console.WriteLine("Done");
        }
    }
}
