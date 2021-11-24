using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions.ILProcessor;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.World.IO
{
	/// <summary>
	/// Adds a Terraria.WorldGen.autoSave check in saveAndPlay
	/// </summary>
	public class SaveAndPlay : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.3.2, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Adding autoSave check to saveAndPlay";
		public override void Run()
		{
			var vanilla = this.Method(() => Terraria.WorldGen.saveAndPlay());
			var autoSave = this.Field(() => Terraria.Main.autoSave);

			var first_instruction = vanilla.Body.Instructions.First();

			vanilla.Body.GetILProcessor().InsertBefore(first_instruction, 
				new { OpCodes.Ldsfld, autoSave },
				new { OpCodes.Brtrue, first_instruction },
				new { OpCodes.Ret }
			);
		}
	}
}
