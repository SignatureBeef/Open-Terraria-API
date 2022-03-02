using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Extensions.ILProcessor;
using OTAPI.Patcher.Engine.Modification;
using System;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.World.IO
{
	public class Load : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.3.6, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking WorldFile.loadWorld(bool)...";
		public override void Run()
		{
			var vanilla = this.Method(() => Terraria.IO.WorldFile.LoadWorld(false));

			bool tmp = false;
			var cbkBegin = this.Method(() => OTAPI.Callbacks.Terraria.WorldFile.LoadWorldBegin(ref tmp));
			var cbkEnd = this.Method(() => OTAPI.Callbacks.Terraria.WorldFile.LoadWorldEnd(tmp));

			vanilla.Wrap
			(
				beginCallback: cbkBegin,
				endCallback: cbkEnd,
				beginIsCancellable: true,
				noEndHandling: false,
				allowCallbackInstance: false
			);

			var nameId = 0;
			foreach (var handler in vanilla.Body.ExceptionHandlers.Skip(1).Where(x => x.HandlerType == ExceptionHandlerType.Catch))
			{
				var exType = this.SourceDefinition.MainModule.Import(
					typeof(Exception)
				);
				var exVariable = new VariableDefinition($"exceptionObject{++nameId}", exType);

				vanilla.Body.Variables.Add(exVariable);

				handler.CatchType = this.SourceDefinition.MainModule.Import(
					typeof(Exception)
				);

				handler.HandlerStart.OpCode = OpCodes.Stloc;
				handler.HandlerStart.Operand = exVariable;

				var processor = vanilla.Body.GetILProcessor();
				processor.InsertAfter(handler.HandlerEnd.Previous(x => x.OpCode == OpCodes.Stsfld
					&& (x.Operand as FieldReference).Name == "loadSuccess"),
					new { OpCodes.Ldloc, exVariable },
					new
					{
						OpCodes.Call,
						Operand = this.SourceDefinition.MainModule.Import(
						typeof(System.Console).GetMethods().Single(x => x.Name == "WriteLine"
							&& x.GetParameters().Count() == 1
							&& x.GetParameters()[0].ParameterType.Name == "Object"
						)
					)
					}
				);
			}
		}
	}
}
