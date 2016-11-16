using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Extensions.ILProcessor;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.World
{
	/// <summary>
	/// This modification will insert a callback right before the 'stopDrops = true`
	/// in the WorldGen.meteor method so we can create a hook for when meteors drop.
	/// </summary>
	public class DropMeteor : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.4.1, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking WorldGen.meteor(x,y)...";
		public override void Run()
		{
			//Get the vanilla meteor method reference
			var vanilla = SourceDefinition.Type("Terraria.WorldGen").Method("meteor");

			//Get the OTAPI callback method reference
			int tmp = 0;
			var callback = this.SourceDefinition.MainModule.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.WorldGen.DropMeteor(ref tmp, ref tmp))
			);

			//Look for stopDrops = true in the vanilla methods instructions
			var stopDrops = vanilla.Body.Instructions.Single(instruction =>
				 instruction.OpCode == OpCodes.Stsfld
				&& instruction.Operand is FieldReference
				&& (instruction.Operand as FieldReference).Name == "stopDrops"

				&& instruction.Previous.OpCode == OpCodes.Ldc_I4_1
			);

			//Get the IL processor instance so we can inject some il
			var processor = vanilla.Body.GetILProcessor();

			//Get the instruction reference in which we use to continue
			//on with vanilla code if our callback doesn't cancel the method.
            //Since we are inserting before the stopDrops = true, we use the
            //first instruction for the setter, which is ldc.i4.1 (which means (bool)true)
			var insContinue = stopDrops.Previous;

			//Inject the callback IL
			processor.InsertBefore(insContinue,
				//Create the callback execution
				new { OpCodes.Ldarga, Operand = vanilla.Parameters[0] }, //reference to int x
				new { OpCodes.Ldarga, Operand = vanilla.Parameters[1] }, //reference to int y
				new { OpCodes.Call, callback }, //OTAPI.Callbacks.Terraria.WordGen.DropMeteor(ref x, ref y)

				//If the callback is not canceled, continue on with vanilla code
				new { OpCodes.Brtrue_S, insContinue },

				//If the callback is canceled, return false
				new { OpCodes.Ldc_I4_0 }, //false
				new { OpCodes.Ret } //return
			);

            /* We now should have code in meteor looking like:
                    }
                }
				if (!WorldGen.DropMeteor(ref i, ref j))
				{
					return false;
				}
				WorldGen.stopDrops = true;
				num = WorldGen.genRand.Next(17, 23);
			*/
        }
    }
}
