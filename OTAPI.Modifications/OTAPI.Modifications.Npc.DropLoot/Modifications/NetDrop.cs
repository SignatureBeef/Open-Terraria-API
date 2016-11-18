using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Npc
{
	public class DropLoot_1_NetDrop : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.4.3, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Npc.NPCLoot\\SendData...";

		public override void Run()
		{
			var npcLoot = SourceDefinition.Type("Terraria.NPC").Method("NPCLoot");

			//In the NPCLoot method there is a call to send packet 88 (after item drop).
			//We will also want to hook this in the case the returned value from DropLoot
			//cancels the event.
			//Note, each update will need checking for any other calls to DropLoot that populate variables
			//as currently this is the only one.
			//TODO: write a test for this

			var il = npcLoot.Body.GetILProcessor();

			//This section will add '&& num40 >= 0' to the statement above "Main.item [num40].color = this.color;"
			var insColour = npcLoot.Body.Instructions.Single(x => x.OpCode == OpCodes.Ldfld && x.Operand == SourceDefinition.Type("Terraria.NPC").Field("color")); //Grab where the call is located
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
		}
	}
}
