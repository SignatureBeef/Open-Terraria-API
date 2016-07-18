using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Extensions.ILProcessor;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.World
{
	public class TileUpdate : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.1.1, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking hardmode tile updates...";
		public override void Run()
		{
			//Get the vanilla method reference
			var vanilla = SourceDefinition.Type("Terraria.WorldGen").Method("hardUpdateWorld");

			//Get the OTAPI callback method reference
			//int tmp = 0;
			var callback = this.SourceDefinition.MainModule.Import(
				this.Method(() => OTAPI.Core.Callbacks.Terraria.WorldGen.HardmodeTileUpdate(0, 0, 0))
			);

			/* In this particular tile update mod (part 1) we are 
			 * looking specifically for each occurrence of the below
			 * pair of methods.
			 * Above the if blocks, we insert callbacks
			 
				WorldGen.SquareTileFrame(num11, num12, true);
				NetMessage.SendTileSquare(-1, num11, num12, 1);
			 */

			var targets = vanilla.Body.Instructions.Where(instruction =>
				instruction.OpCode == OpCodes.Stfld
				&& (instruction.Operand as FieldReference).Name == "type"
				&& (instruction.Operand as FieldReference).DeclaringType.FullName == "Terraria.Tile"
			).ToArray();

			var processor = vanilla.Body.GetILProcessor();

			foreach (var insertionPoint in targets)//.Take(1))
			{
				//Replace field update, Terraria.Main.tile[x,y].type = <value>
				//with our callback, so it receives the x, y, and type.
				insertionPoint.OpCode = OpCodes.Call;
				insertionPoint.Operand = callback;

				//We now need to remove the tile instance IL, as we only need the x,y pos
				//This means the ldsfld to the tile array, and the Get accessor
				//[Part 1] Starting with the simple Get accessor, it can simply be removed as there
				//are no instruction references to it as its in the middle of items on the stack
				var getAccessor = insertionPoint.Previous(ins =>
					ins.OpCode == OpCodes.Call
					&& (ins.Operand as MethodReference).Name == "Get"
				);
				processor.Remove(getAccessor);

				//[Part 2] Remove the tile array load instruction.
				//To do this we need to replace all transfers to the indexer arguments
				//otherwise the if blocks will be ruined.
				var tileLoader = insertionPoint.Previous(ins =>
					ins.OpCode == OpCodes.Ldsfld
					&& (ins.Operand as FieldReference).Name == "tile"
				);

				//Replace all instruction references from the instruction we are removing,
				//onto the following instruction
				tileLoader.ReplaceTransfer(tileLoader.Next, vanilla);

				//It's safe to remove the tile load instruction now
				processor.Remove(tileLoader);

				processor.InsertAfter(insertionPoint,
					new { OpCodes.Brtrue_S, insertionPoint.Next },
					new { OpCodes.Ret }
				);
			}
		}
	}
}
