using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Extensions.ILProcessor;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Item
{
	public class QuickStack : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null"
		};

		public override string Description => "Hooking Chest.PutItemInNearbyChest...";
		public override void Run()
		{
			var vanilla = this.Type<Terraria.Chest>().Method("PutItemInNearbyChest");
			var callback = this.SourceDefinition.MainModule.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.Chest.QuickStack(0, null, 0))
			);

			var processor = vanilla.Body.GetILProcessor();

			var beginInstruction = vanilla.Body.Instructions.Single(x => x.OpCode == OpCodes.Bge_Un);
			var endInstruction = beginInstruction.Next(x => x.OpCode == OpCodes.Ldc_I4_0);

			processor.InsertAfter(beginInstruction,
				new { OpCodes.Ldarg, Operand = vanilla.Parameters.Skip(2).SingleOrDefault() },
				new { OpCodes.Ldarg, Operand = vanilla.Parameters.First() },
				new { OpCodes.Ldloc_0 },
				new { OpCodes.Call, callback },
				new { OpCodes.Brtrue, endInstruction },
				new { OpCodes.Br, Operand = (Instruction)beginInstruction.Operand }
			);
		}
	}
}
