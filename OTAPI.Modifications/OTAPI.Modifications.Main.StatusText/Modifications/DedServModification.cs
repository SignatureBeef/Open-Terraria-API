using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Modifications.StatusText.Modifications
{
	/// <summary>
	/// The purpose of this modification is to remove all Console.WriteLines*
	/// from a Main.statusText change.
	/// Instead, this will be handled by our SetStatusText callback.
	/// </summary>
	public class DedServModification : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.0.0, Culture=neutral, PublicKeyToken=null"
		};

		public override string Description => "Removing statusText console writes";

		public override void Run()
		{
			//Get the Terraria.Main.DedServ method reference so we can open it up
			//and modify it's IL
			var mthDedServ = this.Method(() => (new Terraria.Main()).DedServ());

			//Get the il processor for the DedServ method
			var processor = mthDedServ.Body.GetILProcessor();

			//Currently the code that Re-Logic has for each of the Console.WriteLine
			//branches are the same when comparing, and is marked below with *
			//	*	if (Main.oldStatusText != Main.statusText)
			//		{
			//			Main.oldStatusText = Main.statusText;
			//			Console.WriteLine(Main.statusText);
			//      }
			//This mean we can easily isolate the branches we want to remove by looking
			//for the two variable loads (oldStatusText, statusText) and the inequality
			//comparator method.
			//
			//Below you can see the IL we want to be looking for marked with *.
			//We then want to remove all instructions in the entire branch from these
			//instruction onward (except the oldStatusText as we use it to inject a callback). 
			//To do this we can use the instruction reference in the brfalse.s (marked with ^)
			//so we know which instruction to stop at (marked with >).
			//	*	IL_0545: ldsfld string Terraria.Main::oldStatusText
			//	*	IL_054a: ldsfld string Terraria.Main::statusText
			//	*	IL_054f: call bool[mscorlib] System.String::op_Inequality(string, string)
			//	^	IL_0554: brfalse.s IL_056a

			//		IL_0556: ldsfld string Terraria.Main::statusText
			//		IL_055b: stsfld string Terraria.Main::oldStatusText
			//		IL_0560: ldsfld string Terraria.Main::statusText
			//		IL_0565: call void[mscorlib] System.Console::WriteLine(string)

			//	>	IL_056a: ldsfld int32 Terraria.Main::menuMode
			//		IL_056f: ldc.i4.s 10
			//		IL_0571: beq.s IL_0545

			//Get the field references to the fields that we are looking for in the comparison
			//so we can check which instructions are using them. 
			//Alternatively below, we could cast a particular instructions operand to a field reference
			//and check the field name and the declaring type (similar to the op_Inequality instruction
			//check).
			var fldOldStatusText = this.Field(() => Terraria.Main.oldStatusText);
			var fldStatusText = this.Field(() => Terraria.Main.statusText);

			//Get the UpdateStatusText callback that we will replace the ldsfld oldStatusText
			//instruction with (this saves reinjecting and updating instruction references)
			var mthUpdateStatusText = this.SourceDefinition.MainModule.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.Main.UpdateStatusText())
			);

			//For each branch that match our target oldStatusText != statusText
			foreach (var instruction in mthDedServ.Body.Instructions
				.Where(ins =>

					//Check for the oldStatusText field load instruction
					ins.OpCode == OpCodes.Ldsfld && ins.Operand == fldOldStatusText

					//Check to see if the next instruction is the statusText field load instruction
					&& ins.Next.OpCode == OpCodes.Ldsfld && ins.Next.Operand == fldStatusText

					//Check if the two fields are being compared using the inequality operator (ie !=)
					&& ins.Next.Next.OpCode == OpCodes.Call
						&& ins.Next.Next.Operand is MethodReference
						&& (ins.Next.Next.Operand as MethodReference).Name == "op_Inequality"

					//Check to see if the brfalse.s is here as we expected
					&& ins.Next.Next.Next.OpCode == OpCodes.Brfalse_S
						&& ins.Next.Next.Next.Operand is Instruction
				)

				//Take a clone of the array so that the processor.Remove calls don't
				//cause issues with our foreach
				.ToArray()
			)
			{
				var insEndOfBranch = instruction.Next.Next.Next.Operand as Instruction;

				//Remove the branch, keeping the current so we can keep looping
				//and so we can update it to our callback next
				while (instruction.Next != insEndOfBranch)
					processor.Remove(instruction.Next);

				//Update the oldStatusText instruction to our callback
				//So we can perform vanilla functionality but allow
				//upstream consumers to cancel.
				instruction.OpCode = OpCodes.Call;
				instruction.Operand = mthUpdateStatusText;
			}
		}
	}
}
