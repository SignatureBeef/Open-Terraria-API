using Mono.Cecil;
using Mono.Cecil.Cil;
using NDesk.Options;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Npc
{
	public class DropLoot_1_DropLoot : ModificationBase
	{
		public override string Description => "Creating DropLoot";
        public override void Run()
		{
			var newItem = SourceDefinition.Type("Terraria.Item").Method("NewItem");

			//In this patch we create a custom DropLoot method that will be the receiver
			//of all Item.NewItem calls in NPCLoot.


			//Create the new DropLoot call in the Terraria.NPC class
			var dropLoot = new MethodDefinition("DropLoot", MethodAttributes.Public | MethodAttributes.Static, newItem.ReturnType);
			SourceDefinition.Type("Terraria.NPC").Methods.Add(dropLoot);

			var il = dropLoot.Body.GetILProcessor();

			//Clone the parameters from the Item.NewItem method (with no byreference)
			foreach (var prm in newItem.Parameters)
				dropLoot.Parameters.Add(prm);

			//Collect the hooks
			var apiMatch = ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.Npc").Methods.Where(x => x.Name.StartsWith("DropLoot"));
			if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching OnDropLoot Begin/End calls in the API");
			var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
			var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));

			//Create the value to hold the new item id
			var vrbItemId = new VariableDefinition("otaItem", (cbkBegin.Parameters[0].ParameterType as ByReferenceType).ElementType);
			dropLoot.Body.Variables.Add(vrbItemId);

			il.Emit(OpCodes.Ldloca_S, vrbItemId); //Loads our variable by reference so our callback and alter it.
			var beginResult = dropLoot.EmitBeginCallback(cbkBegin, false, false, false);

			//Inject the begin call
			var insFirstForMethod = dropLoot.EmitMethodCallback(newItem, false, false);
			//Store the result into our new variable
			il.Emit(OpCodes.Stloc, vrbItemId);

			//Set the vanilla instruction to be resumed upon continuation of the begin hook.
			if (beginResult != null && beginResult.OpCode == OpCodes.Pop)
			{
				beginResult.OpCode = OpCodes.Brtrue_S;
				beginResult.Operand = insFirstForMethod;

				//Emit the cancellation return IL
				il.InsertAfter(beginResult, il.Create(OpCodes.Ret));
				il.InsertAfter(beginResult, il.Create(OpCodes.Ldloc, vrbItemId));
			}

			//Inject the end callback
			dropLoot.EmitEndCallback(cbkEnd, false, false);

			//Emit the return value using the result variable we injected
			il.Emit(OpCodes.Ldloc, vrbItemId);
			il.Emit(OpCodes.Ret);
		}
	}
}
