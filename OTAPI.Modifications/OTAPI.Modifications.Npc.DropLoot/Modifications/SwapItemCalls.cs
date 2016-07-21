using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Npc
{
	public class DropLoot_1_SwapItemCalls : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.2.1, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Npc.NPCLoot\\NewItem...";
		public override void Run()
		{
			var npcLoot = SourceDefinition.Type("Terraria.NPC").Method("NPCLoot");
			var dropLoot = SourceDefinition.Type("Terraria.NPC").Method("DropLoot");

			//In this patch we swap all Item.NewItem calls in NPCLoot to use our previously
			//created DropLoot method

			//Gather all Item.NewItem calls
			var itemCalls = npcLoot.Body.Instructions.Where(x => x.OpCode == OpCodes.Call
								&& x.Operand is MethodReference
								&& (x.Operand as MethodReference).Name == "NewItem"
								&& (x.Operand as MethodReference).DeclaringType.Name == "Item").ToArray();

			//Swap each Item.NewItem calls to our Npc.DropLoot method.
			foreach (var call in itemCalls)
				call.Operand = dropLoot;
		}
	}
}
