using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Npc
{
	public class Killed : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.4.3, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Npc.checkDead";
		public override void Run()
		{
			/*
			 * Here we will look the the `this.active = false;` in the Terraria.NPC.checkDead
			 * method so we can inject a callback before it to notify that the NPC is finally
			 * killed.
			 */

			var checkDead = this.Method(() => (new Terraria.NPC()).checkDead());
			var callback = this.SourceDefinition.MainModule.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.Npc.Killed(null))
			);
			
			var ins = checkDead.Body.Instructions.Where(x =>
				//Check to see if we are updating a variable
				x.OpCode == OpCodes.Stfld

				//Ensure the variable is the .active field
				&& x.Operand is FieldReference
				&& (x.Operand as FieldReference).Name == "active"

				//Ensure the value being set is 'false'
				&& x.Previous.OpCode == OpCodes.Ldc_I4_0
			)
				.Single() //Get the only this.active = false
				.Previous //Go back to ldc.i4.0
				.Previous; //Go back to ldarg.0 (this) we can insert before
			
			var processor = checkDead.Body.GetILProcessor();

			//Add the npc instance to the stack (this)
			processor.InsertAfter(ins, processor.Create(OpCodes.Ldarg_0));

			//Call our callback
			processor.InsertAfter(ins, processor.Create(OpCodes.Call, callback));
		}
	}
}
