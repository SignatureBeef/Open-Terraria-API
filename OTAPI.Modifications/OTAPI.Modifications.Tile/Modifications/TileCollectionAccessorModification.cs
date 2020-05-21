using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Collections.Generic;

namespace OTAPI.Modification.Tile.Modifications
{
	public class TileCollectionAccessorModification : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.0.3, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Patching tile collections accessors...";

		public override void Run()
		{
			//The process here is to look through each instruction in the assembly
			//for array accessors for Terraria.Tile[,]. Specifically ones that relate back to
			//Terraria.Main.tile, Terraria.World.Generation.GenBase._tiles and Terraria.GameContent.Liquid.LiquidRenderer._tiles
			//
			//Once we find a Get or Set call, we are to swap to them out to ITileCollection.get_Item/set_Item.
			//
			//Here is a bit of IL that we are looking for

			//Getter:
			//		IL_0108: ldloc.s 5
			//		IL_010a: ldloc.s 6
			//	**	IL_010c: call instance class Terraria.Tile class Terraria.Tile[0..., 0...]::Get(int32, int32)
			//
			//Setter:
			//		IL_003f: ldarg.0
			//		IL_0040: ldarg.1
			//		IL_0041: ldloc.0
			//	*	IL_0042: call instance void class Terraria.Tile[0..., 0...]::Set(int32, int32, class Terraria.Tile)
			//
			//The instructions marked with * are the ones that we are looking to swap.

			//Grab the iTileCollection type definition
			var iTileCollection = this.Type<OTAPI.Tile.ITileCollection>();

			var mGetTile = this.SourceDefinition.MainModule.Import(iTileCollection.Method("get_Item"));
			var mSetTile = this.SourceDefinition.MainModule.Import(iTileCollection.Method("set_Item"));

			this.SourceDefinition.MainModule.ForEachInstruction((method, instruction) =>
			{
				var callMethod = instruction.Operand as MethodReference;
				if (callMethod != null)
				{
					var arrayType = callMethod.DeclaringType as ArrayType;
					if (arrayType != null && arrayType.ElementType.FullName == "Terraria.Tile")
					{
						if (callMethod.Name == "Get")
						{
							instruction.OpCode = OpCodes.Callvirt;
							instruction.Operand = mGetTile;
						}
						else if (callMethod.Name == "Set")
						{
							instruction.OpCode = OpCodes.Callvirt;
							instruction.Operand = mSetTile;
						}
					}
				}
			});
		}
	}
}
