using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Extensions.ILProcessor;
using OTAPI.Patcher.Engine.Modification;
using System;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Item
{
	public class ServerPlace : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.4.1, Culture=neutral, PublicKeyToken=null"
		};

		public override string Description => "Hooking Chest.ServerPlaceItem...";
		public override void Run()
		{
			//Add an extra Player argument to the PutItemInNearbyChest method.
			//Then add the Player instance IL an any callers.
			var mthServerPlaceItem = this.Type<Terraria.Chest>().Method("PutItemInNearbyChest");

			mthServerPlaceItem.Parameters.Add(new ParameterDefinition(
				"playerId",
				 ParameterAttributes.None,
				this.TypeSystem.Int32
			));

			ILProcessor processor;

			this.SourceDefinition.MainModule.ForEachInstruction((method, instruction) =>
			{
				var call = instruction.Operand as MethodReference;
				if (call != null && call.Name == "PutItemInNearbyChest")
				{
					switch (method.Name)
					{
						case "ServerPlaceItem":
							//Add the player id from the first argument
							processor = method.Body.GetILProcessor();

							processor.InsertBefore(instruction, new
							{
								OpCodes.Ldarg_0
							});
							break;
						case "QuickStackAllChests":
							processor = method.Body.GetILProcessor();

							processor.InsertBefore(instruction,
								new { OpCodes.Ldarg_0, },
								new { OpCodes.Ldfld, Operand = (FieldReference)this.Field(() => (new Terraria.Player()).whoAmI) }
							);

							method.Body.SimplifyMacros();
							break;
						default:
							throw new NotImplementedException($"{method.Name} is not a supported caller for this modification");
					}
				}
			});
		}
	}
}
