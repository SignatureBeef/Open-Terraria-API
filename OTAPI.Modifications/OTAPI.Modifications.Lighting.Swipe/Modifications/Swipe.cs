using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Main
{
	public class Swipe : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"Terraria, Version=1.3.3.2, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Lighting.Swipe...";

		public override void Run()
		{
			//Grab the methods
			var tLighting = this.SourceDefinition.Type("Terraria.Lighting");

			var mCallback = this.SourceDefinition.MainModule.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.Lighting.Swipe(null))
			);

			foreach (var methodName in new[] { "callback_LightingSwipe", "doColors" })
			{
				var mVanilla = tLighting.Method(methodName);

				var processor = mVanilla.Body.GetILProcessor();

				//Find all occurrences to the function action and replace it with out callback.
				//Back track to each Lighting.LightingSwipeData::function instruction and remove it and the instruction before it

				foreach (var mInvoke in mVanilla.Body.Instructions
						.Where(ins => ins.OpCode == OpCodes.Callvirt
							&& (ins.Operand as MethodReference).Name == "Invoke")
						.ToArray()
				)
				{
					mInvoke.OpCode = OpCodes.Call;
					mInvoke.Operand = mCallback;

					var aFunction = mInvoke.Previous(nins => nins.OpCode == OpCodes.Ldfld
						&& (nins.Operand as FieldReference).Name == "function"
					);

					processor.Remove(aFunction.Previous);
					processor.Remove(aFunction);
				}
			}
		}
	}
}
