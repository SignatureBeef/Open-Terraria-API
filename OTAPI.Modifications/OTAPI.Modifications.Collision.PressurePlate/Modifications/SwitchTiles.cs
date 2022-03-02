using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Collision
{
	/// <summary>
	/// This mod we will replace the HitSwitch calls with our own custom version, as the SendData
	/// method will also be ripped out as it's quicker and easier to do this than hacking ourselves
	/// into the if blocks.
	/// </summary>
	public class SwitchTiles : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.3.6, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => @"Hooking Collision.SwitchTiles\PressurePlate...";

		public override void Run()
		{
			var vanilla = this.SourceDefinition.Type("Terraria.Collision").Method("SwitchTiles");
			var callback = vanilla.Module.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.Collision.PressurePlate(0, 0, null))
			);
			var il = vanilla.Body.GetILProcessor();

			//Find all HitSwitch calls
			var calls = vanilla.Body.Instructions.Where(x => x.OpCode == OpCodes.Call
											 && x.Operand is MethodReference
											 && (x.Operand as MethodReference).Name == "HitSwitch");

			//Now action each call individually
			foreach (var call in calls.ToArray()) //We need our own copy here, so transform into an array
			{
				//It's already a call, so just swap out the method reference
				call.Operand = callback;

				//Now we insert the entity argument to our custom version
				var prmEntity = vanilla.Parameters.Single(x => x.ParameterType.Name == "IEntity");
				il.InsertBefore(call, il.Create(OpCodes.Ldarg, prmEntity));

				//Time to remove SendData calls under the HitSwitch calls.
				//I wasn't kidding...not to worry, it's reimplemented in the core.
				var stopAt = call.Next(x => x.OpCode == OpCodes.Call).Next;
				for (;;)
				{
					il.Remove(call.Next);
					if (call == stopAt.Previous) break;
				}
			}
		}
	}
}
