using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Collections.Generic;
using System.Linq;

namespace OTAPI.Modifications.Input.Filter.Modifications
{
	public class InputTextModification : ModificationBase
	{
		public override IEnumerable<string> AssemblyTargets => new[]
		{
			"Terraria, Version=1.3.4.3, Culture=neutral, PublicKeyToken=null"
		};

		public override string Description => "Hooking keyboard filters";

		public override void Run()
		{
			var mth = this.SourceDefinition.Type("Terraria.keyBoardInput").StaticConstructor();

			var ins = mth.Body.Instructions.Single(x =>
				x.OpCode == OpCodes.Newobj
			);

			ins.OpCode = OpCodes.Call;
			ins.Operand = this.SourceDefinition.MainModule.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.keyBoardInput.CreateMessageFilter())
			);
		}
	}
}
