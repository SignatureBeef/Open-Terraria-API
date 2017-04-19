using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Extensions.ILProcessor;
using OTAPI.Patcher.Engine.Modification;
using System;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Net
{
	public class ReceiveDataException : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.5.0, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking NetMessage.CheckBytes\\TryCatch";

		public override void Run()
		{
			var vanilla = this.Method(() => Terraria.NetMessage.CheckBytes(0));

			var handler = vanilla.Body.ExceptionHandlers.Single(x => x.HandlerType == ExceptionHandlerType.Catch);

			var exType = this.SourceDefinition.MainModule.Import(
				typeof(Exception)
			);
			var exVariable = new VariableDefinition("exceptionObject", exType);

			vanilla.Body.Variables.Add(exVariable);

			handler.CatchType = this.SourceDefinition.MainModule.Import(
				typeof(Exception)
			);

			handler.HandlerStart.OpCode = OpCodes.Stloc;
			handler.HandlerStart.Operand = exVariable;
			//Console.WriteLine(handler.CatchType);

			var processor = vanilla.Body.GetILProcessor();
			processor.InsertBefore(handler.HandlerEnd.Previous(x => x.OpCode == OpCodes.Leave_S),
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