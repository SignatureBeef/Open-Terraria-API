using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Extensions.ILProcessor;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.World
{
	public class TilePlace : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.2.1, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking hardmode tile placement...";
		public override void Run()
		{
			//Get the vanilla method reference
			var vanilla = SourceDefinition.Type("Terraria.WorldGen").Method("hardUpdateWorld");

			//Get the OTAPI callback method reference
			//int tmp = 0;
			var callback = this.SourceDefinition.MainModule.Import(
				this.Method(() => OTAPI.Core.Callbacks.Terraria.WorldGen.HardmodeTilePlace(0, 0, 0, false, false, 0, 0))
			);

			/* In this particular hardmode tile mod we replace all WorldGen.PlaceTile
			 * calls to a custom callback version, then replace the Pop instruction
			 * with cancelable IL.
			 */

			var targets = vanilla.Body.Instructions.Where(instruction =>
				instruction.OpCode == OpCodes.Call
				&& (instruction.Operand as MethodReference).Name == "PlaceTile"

				&& instruction.Next.OpCode == OpCodes.Pop
			).ToArray();

			var processor = vanilla.Body.GetILProcessor();

			foreach (var replacementPoint in targets)//.Take(1))
			{
				replacementPoint.Operand = callback;

				replacementPoint.Next.OpCode = OpCodes.Brtrue_S;
				replacementPoint.Next.Operand = replacementPoint.Next.Next;

				processor.InsertAfter(replacementPoint.Next, processor.Create(OpCodes.Ret));
			}
		}
	}
}
