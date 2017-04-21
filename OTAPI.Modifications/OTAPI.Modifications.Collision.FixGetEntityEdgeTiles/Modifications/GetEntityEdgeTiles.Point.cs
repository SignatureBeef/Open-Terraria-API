using Microsoft.Xna.Framework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Collections.Generic;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Collision
{
	public class GetEntityEdgeTilesPoint : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.5.2, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Collision.GetEntityEdgeTiles\\Point...";

		public override void Run()
		{
			var vanilla = this.SourceDefinition.Type("Terraria.Collision").Method("GetEntityEdgeTiles");
			var callback = this.Method(() => OTAPI.Callbacks.Terraria.Collision.GetEntityEdgeTilePoint(0, 0));

			foreach (var instruction in vanilla.Body.Instructions
				.Where(x => x.OpCode == OpCodes.Newobj
					&& (x.Operand as MethodReference).Name == ".ctor"
					&& (x.Operand as MethodReference).DeclaringType.FullName == "Microsoft.Xna.Framework.Point"
				)
				.ToArray())
			{
				instruction.OpCode = OpCodes.Call;
				instruction.Operand = this.SourceDefinition.MainModule.Import(callback);
			}
		}
	}
}