using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Main
{
	public class GameStarted : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.0.3, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Game.DedServ...";

		public override void Run()
		{
			//Grab the Initialise method manually since it's protected
			var vanilla = this.SourceDefinition.Type("Terraria.Main").Method("DedServ");

			var callback = this.SourceDefinition.MainModule.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.Main.GameStarted())
			);

			var processor = vanilla.Body.GetILProcessor();

			var insertionOffset = vanilla.Body.Instructions.Single(
				x => x.OpCode == OpCodes.Call
					&& ((MethodReference)x.Operand).Name == "startDedInput"
			);

			var insCallback = Instruction.Create(OpCodes.Call, callback);
			processor.InsertBefore(insertionOffset, insCallback);

			insertionOffset.ReplaceTransfer(insCallback, vanilla);
		}
	}
}
