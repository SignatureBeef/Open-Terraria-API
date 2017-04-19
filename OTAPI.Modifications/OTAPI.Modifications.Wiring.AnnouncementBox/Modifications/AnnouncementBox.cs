using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Extensions.ILProcessor;
using OTAPI.Patcher.Engine.Modification;
using System;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.World
{
	public class AnnouncementBox : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.5.0, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Wiring.HitWireSingle(x,y)...";
		public override void Run()
		{
			//Get the vanilla meteor method reference
			var vanilla = this.SourceDefinition.Type("Terraria.Wiring").Method("HitWireSingle");

			if (vanilla.Parameters.Count != 2)
			{
				throw new NotSupportedException("Expected 2 parameters for the callback");
			}

			//Get the OTAPI callback method reference
			var callback = this.SourceDefinition.MainModule.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.Wiring.AnnouncementBox(0, 0, 0))
			);

			var insertionPoint = vanilla.Body.Instructions.First(
				x => x.OpCode == OpCodes.Ldsfld
				&& (x.Operand as FieldReference).Name == "AnnouncementBoxRange"
			);

			var signVariable = vanilla.Body.Instructions.First(
				x => x.OpCode == OpCodes.Call
				&& (x.Operand as MethodReference).Name == "ReadSign"
			).Next.Operand;

			var injectedInstructions = vanilla.Body.GetILProcessor().InsertBefore(insertionPoint,
				new { OpCodes.Ldarg_0 },
				new { OpCodes.Ldarg_1 },
				new { OpCodes.Ldloc_S, Operand = signVariable as VariableDefinition },
				new { OpCodes.Call, callback },
				new { OpCodes.Brtrue_S, insertionPoint },
				new { OpCodes.Ret }
			);

			insertionPoint.ReplaceTransfer(injectedInstructions[0], vanilla);

			injectedInstructions.Single(x => x.OpCode == OpCodes.Brtrue_S).Operand = insertionPoint;
		}
	}
}
