using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System;
using System.Linq;

namespace OTAPI.Modification.Tile.Modifications
{
	/// <summary>
	/// This modification is used to move all tile property/field updates
	/// from the constructor out into a virtual Initialise method.
	/// This gives consumers more control over the default tile mechanism.
	/// </summary>
	[Ordered(8)]
	public class TileConstructor : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.4.3, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.4.3, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Adding Initialise to Terraria.Tile...";

		public override void Run()
		{
			//Get the Terraria.Tile method definition
			var terrariaTile = this.SourceDefinition.Type("Terraria.Tile");

			//Create and add the new Tile method that will have the real constructor instructions.
			var mInitialise = new MethodDefinition("Initialise",
				MethodAttributes.Public | MethodAttributes.NewSlot | MethodAttributes.Virtual,
				this.SourceDefinition.MainModule.TypeSystem.Void
			);
			terrariaTile.Methods.Add(mInitialise);

			//Find the tile reference that Terraria uses to create tiles with
			var ctor = terrariaTile.Methods.Single(x => x.Name == ".ctor" && !x.HasParameters);

			//Get the IL processor instance so we can modify the tile constructors il
			var il = ctor.Body.GetILProcessor();

			//Get all constructor instructions, skipping the first two as they are for
			//the underlying object. ie object.ctor
			var instructions = ctor.Body.Instructions.Skip(2)
				.ToArray(); //Get a clone

			//Remove the instructions from the constructor
			foreach (var instruction in instructions.Reverse())
				ctor.Body.Instructions.Remove(instruction);

			//Add the instructions into the new method
			foreach (var instruction in instructions)
				mInitialise.Body.Instructions.Add(instruction);

			//Add the new method call into the Tile construction
			ctor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
			ctor.Body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, mInitialise));

			//Add the return instruction as per method requirements. We removed the existing one and put
			//it in the new method, so we need to put a new one back into the constructor too.
			ctor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
		}
	}
}
