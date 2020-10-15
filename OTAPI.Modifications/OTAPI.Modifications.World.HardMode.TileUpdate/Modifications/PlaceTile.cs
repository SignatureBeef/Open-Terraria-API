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
			"TerrariaServer, Version=1.4.1.1, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking hardmode tile placement...";
		public override void Run()
		{
			//Get the vanilla method reference
			var vanilla = this.Method(() => Terraria.WorldGen.hardUpdateWorld(0, 0));

			//Get the OTAPI callback method reference
			var callback = this.SourceDefinition.MainModule.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.WorldGen.HardmodeTilePlace(0, 0, 0, false, false, 0, 0))
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

				var insContinue = replacementPoint.Next.Next.Next(i =>
					i.OpCode == OpCodes.Call
					&& (i.Operand as MethodReference).Name == "SendTileSquare"
				).Next;

				replacementPoint.Next.OpCode = OpCodes.Brtrue_S;
				replacementPoint.Next.Operand = insContinue;

				processor.InsertAfter(replacementPoint.Next, processor.Create(OpCodes.Ret));
			}
		}
	}
}
