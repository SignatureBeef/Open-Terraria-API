using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

using System;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Npc
{
	/// <summary>
	/// In this patch we will replace the server code to use a custom Item.NewItem method named "BossBagItem".
	/// We then insert some IL to check the result variable for the int -1. If they match
	/// then we cancel the vanilla function by returning.
	/// </summary>
	public class BossBagItem : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.1.1, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Npc.DropBossBag\\Item...";

		public override void Run()
		{
			var vanilla = SourceDefinition.Type("Terraria.NPC").Method("DropItemInstanced");
			var callback = this.Method(
				() => OTAPI.Callbacks.Terraria.Npc.BossBagItem(0, 0, 0, 0, 0, 0, false, 0, false, false, null)
			);

			var il = vanilla.Body.GetILProcessor();

			//Grad the NewItem calls
			var instructions = vanilla.Body.Instructions.Where(x => x.OpCode == OpCodes.Call
																&& x.Operand is MethodReference
																&& (x.Operand as MethodReference).Name == "NewItem"
																&& x.Next.OpCode == OpCodes.Stloc_0);
			//Quick validation check
			if (instructions.Count() != 1) throw new NotSupportedException("Only one server NewItem call expected in DropBossBags.");

			//The first call is in the server block. TODO: client version
			var ins = instructions.First();

			//Swap the NewItem call to our custom item call
			ins.Operand = vanilla.Module.Import(callback);
			//Our argument appends the NPC instance (this) to the arguments
			il.InsertBefore(ins, il.Create(OpCodes.Ldarg_0)); //Instance methods ldarg.0 is the instance object

			//Now we start inserting our own if block to compare the call result.
			var target = ins.Next/*stloc.0*/.Next; //Grabs a reference to the instruction after the stloc.1 opcode so we can insert sequentially
			il.InsertBefore(target, il.Create(OpCodes.Ldloc_0)); //Load the num variable onto the stack
			il.InsertBefore(target, il.Create(OpCodes.Ldc_I4_M1)); //Load -1 onto the stack
			il.InsertBefore(target, il.Create(OpCodes.Ceq)); //Consume & compare the two variables and push 1 (true) or 0 (false) onto the stack
			il.InsertBefore(target, il.Create(OpCodes.Brfalse_S, target)); //if the output of ceq is 0 (false) then continue back on with the [target] instruction. In code terms, if the expression is not -1 then don't exit
			il.InsertBefore(target, il.Create(OpCodes.Ret)); //If we are here, the num2 variable is equal to -1, so we can exit the function.
		}
	}
}
