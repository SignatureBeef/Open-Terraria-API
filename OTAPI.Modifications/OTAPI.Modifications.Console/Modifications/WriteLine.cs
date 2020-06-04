using Mono.Cecil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Patches
{
	public class ConsoleWrites : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.0.5, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking all Console.Write/Line calls...";

		public override void Run()
		{
			return; // no one uses these afaik, and the write/line callbacks need to be 1:1, currently an object is receiving all overloads and thats not ideal
			var redirectMethods = new[]
			{
				"Write",
				"WriteLine"
			};

			SourceDefinition.MainModule.ForEachInstruction((method, instruction) =>
			{
				var mth = instruction.Operand as MethodReference;
				if (mth != null && mth.DeclaringType.FullName == "System.Console")
				{
					if (redirectMethods.Contains(mth.Name))
					{
						var mthReference = this.Resolve(mth);
						if (mthReference != null)
							instruction.Operand = SourceDefinition.MainModule.Import(mthReference);
					}
				}
			});
		}

		MethodReference Resolve(MethodReference method)
		{
			return this.Type<OTAPI.Callbacks.Terraria.Console>()
				.Method(method.Name,
					parameters: method.Parameters,
					acceptParamObjectTypes: true
				);
		}
	}
}
