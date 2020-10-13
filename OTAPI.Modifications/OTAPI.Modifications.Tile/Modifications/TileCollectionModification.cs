using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Modification.Tile.Modifications
{
	public class TileCollectionModification : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.1.0, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Patching tile collections...";

		public override void Run()
		{
			//Import the ITileCollection interface
			var iTileCollection = this.SourceDefinition.MainModule.Import(
				this.Type<OTAPI.Tile.ITileCollection>()
			);

			//Import the ITileCollection Create Hook
			var callback = this.SourceDefinition.MainModule.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.Collection.Create())
			);


			//Currently the only occurrences are:
			//	Terraria.Main.tile
			//	Terraria.World.Generation._tiles
			//
			//I started enumerating each type's properties and fields
			//but thats kind of over kill for whats actually needed.

			//Manually swap the declared types of Tile[] to ITileCollection
			this.Field(() => Terraria.Main.tile).FieldType = iTileCollection;
			this.SourceDefinition.Type("Terraria.WorldBuilding.GenBase").Property("_tiles").PropertyType = iTileCollection;
			this.SourceDefinition.Type("Terraria.GameContent.Liquid.LiquidRenderer").Property("Tiles").PropertyType = iTileCollection;

			//The Terraria.Main.tile's static initialiser also needs to be corrected, as it's initialising
			//the collection with Terraria.Tile[,] still.
			//
			//What we need to do in IL is find where the tile instance is being created with the new array.
			//This is what the IL currently looks like in 1.3.1.1
			//
			//		IL_1fee: stsfld class Terraria.Map.WorldMap Terraria.Main::Map
			//	*	IL_1ff3: ldsfld int32 Terraria.Main::maxTilesX
			//	*	IL_1ff8: ldsfld int32 Terraria.Main::maxTilesY
			//	~	IL_1ffd: newobj instance void class Terraria.Tile[0..., 0...]::.ctor(int32, int32)
			//		IL_2002: stsfld class [OTAPI.Modifications.Tile] OTAPI.Tile.ITileCollection Terraria.Main::tile
			//		IL_2007: ldc.i4 6001
			//		IL_200c: newarr Terraria.Dust
			//
			//The lines on the stack marked with * need to be removed, and a
			//[CALL OTAPI.OTAPI.Callbacks.Terraria.Collection.Create] should replace the instruction at
			//the line marked with ~
			//
			//To do this I will uniquely look for the stsfld opcode by checking the operand to see
			//if it's a field reference to Terraria.Main.tile, and additionally by checking if the previous
			//opcode is newobj.
			//
			//From here I will remove the maxTileX & maxTileY instructions, leaving both newobj and stsfld.
			//The opcode of newobj must then be swapped to CALL and also it's operand to OTAPI's imported callback.

			//Get the static constructor
			var staticConstructor = this.SourceDefinition
				.Type("Terraria.Main")
				.StaticConstructor()
			;

			//Find the stsfld opcode. this will become our callback
			var stsfld = staticConstructor
				.Body
				.Instructions
				.Single(instruction =>
					instruction.OpCode == OpCodes.Stsfld
					&& instruction.Operand is FieldReference
					&& (instruction.Operand as FieldReference).Name == "tile"
					&& (instruction.Operand as FieldReference).DeclaringType.FullName == "Terraria.Main"
					&& instruction.Previous.OpCode == OpCodes.Newobj
				)
			;
			var newobj = stsfld.Previous;

			//Get the IL processor instance so we can modify the IL
			var processor = staticConstructor.Body.GetILProcessor();

			//Remove the maxTilesX,maxTilesY instructions
			for (var x = 0; x < 2; x++)
				processor.Remove(newobj.Previous);

			//Now we can swap the newobj instruction to be our callback.
			newobj.OpCode = OpCodes.Call;
			newobj.Operand = callback;

			//Update return types
			this.SourceDefinition.MainModule.ForEachMethod((method) =>
			{
				var arrayReturnType = method.ReturnType as ArrayType;
				if (arrayReturnType != null && arrayReturnType.ElementType.Name == "Tile")
				{
					method.ReturnType = iTileCollection;
				}
			});
		}
	}
}
