using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Npc
{
	public class Transform : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.2.1, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Npc.Transform...";
		public override void Run()
		{
			var vanilla = SourceDefinition.Type("Terraria.NPC").Method("Transform");
			var callback = ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.Npc").Method("Transform");

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
			il.InsertBefore(insertionPoint, ourEntry = il.Create(OpCodes.Ldarg_0)); //Add the current instance (this in C#) to the callback
			il.InsertBefore(insertionPoint, il.Create(OpCodes.Call, vanilla.Module.Import(callback)));

			//Replace transfers
			insertionPoint.ReplaceTransfer(ourEntry, vanilla);
		}
	}
}
