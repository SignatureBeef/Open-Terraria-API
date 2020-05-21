using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Extensions.ILProcessor;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Npc
{
	public class Transform : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.0.3, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Npc.Transform...";
		public override void Run()
		{
			var vanilla = this.Method(() => (new Terraria.NPC()).Transform(0));
			int tmp = 0;
			var preCallback = vanilla.Module.Import(this.Method(() => OTAPI.Callbacks.Terraria.Npc.PreTransform(null, ref tmp)));
			var postCallback = vanilla.Module.Import(this.Method(() => OTAPI.Callbacks.Terraria.Npc.PostTransform(null)));

			//We could wrap this method, but this hooks is to inform when an npc transforms, not
			//an occurrence of the call.
			//Anyway, this instance we need to insert before the netMode == 2 check.

			//Get the IL processor instance so we can modify IL
			var processor = vanilla.Body.GetILProcessor();

			//Pre callback section
			{
				var first = vanilla.Body.Instructions.First();
				processor.InsertBefore(first,
					new { OpCodes.Ldarg_0 },
					new { OpCodes.Ldarga, Operand = vanilla.Parameters.Single() },
					new { OpCodes.Call, Operand = preCallback },
					new { OpCodes.Brtrue_S, Operand = first },
					new { OpCodes.Ret }
				);
			}

			//Post callback section
			{
				var insertionPoint = vanilla.Body.Instructions.Single(x => x.OpCode == OpCodes.Ldsfld
				   && x.Operand is FieldReference
				   && (x.Operand as FieldReference).Name == "netMode"
				   && x.Next.OpCode == OpCodes.Ldc_I4_2
				);

				//Insert our callback before the if block, ensuring we consider that the if block may be referenced elsewhere
				Instruction ourEntry;
				processor.InsertBefore(insertionPoint, ourEntry = processor.Create(OpCodes.Ldarg_0)); //Add the current instance (this in C#) to the callback
				processor.InsertBefore(insertionPoint, processor.Create(OpCodes.Call, postCallback));

				//Replace transfers
				insertionPoint.ReplaceTransfer(ourEntry, vanilla);
			}
		}
	}
}
