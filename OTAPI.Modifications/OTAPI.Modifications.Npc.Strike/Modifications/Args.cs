using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Extensions.ILProcessor;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Npc
{
	public class Args : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.4.3, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Npc.StrikeNPC(Entity)...";

		public override void Run()
		{
			var vanilla = this.Method(() => (new Terraria.NPC()).StrikeNPC(0, 0, 0, false, false, false));

			vanilla.Parameters.Add(
				new Mono.Cecil.ParameterDefinition("entity",
				Mono.Cecil.ParameterAttributes.HasDefault | ParameterAttributes.Optional,

					this.SourceDefinition.MainModule.Import(this.Type<Terraria.Entity>())
				)
				{
					Constant = null
				}
			);

			this.SourceDefinition.MainModule.ForEachInstruction((method, instruction) =>
			{
				var methodReference = instruction.Operand as MethodReference;
				if (methodReference != null && methodReference.FullName == vanilla.FullName)
				{
					var processor = method.Body.GetILProcessor();

					if (method.Name == "GetData")
					{
						processor.InsertBefore(instruction,
							new { OpCodes.Ldsfld, Operand = this.Field(() => Terraria.Main.player) },
							new { OpCodes.Ldarg_0 },
							new { OpCodes.Ldfld, Operand = this.Field(() => (new Terraria.MessageBuffer()).whoAmI) },
							new { OpCodes.Ldelem_Ref }
						);
						//processor.InsertAfter(instruction,
						//	new { OpCodes.Ldarg_0 },
						//	new { OpCodes.Ldfld, Operand = this.Field(() => (new Terraria.MessageBuffer()).whoAmI) },
						//	new
						//	{
						//		OpCodes.Call,
						//		Operand = this.SourceDefinition.MainModule.Import(
						//			typeof(System.Console).GetMethods().Single(x => x.Name == "WriteLine"
						//				&& x.GetParameters().Count() == 1
						//				&& x.GetParameters()[0].ParameterType.Name == "Int32"
						//			))
						//	}
						//);
					}
					else processor.InsertBefore(instruction, processor.Create(OpCodes.Ldnull));
				}
			});
		}
	}
}