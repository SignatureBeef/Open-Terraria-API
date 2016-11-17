using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Collections.Generic;
using System.Linq;

namespace OTAPI.Modifications.Mono.Modifications
{
	/// <summary>
	/// Mono does not like the SetConsoleCtrlHandler method that terraria uses for its server.
	/// This modification will remove the call to this method, allowing mono to continue.
	/// 
	/// Alternatively we could have used <see cref="Program.LaunchGame"/>, but we would then
	/// have to implement the assembly resolving (at least while terraria still contains
	/// embedded assemblies)
	/// </summary>
	public class RemoveConsoleCtrlCheck : ModificationBase
	{
		public override IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.4.2, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Removing console ctrl check";

		public override void Run()
		{
			//Get the WindowsLaunch Main method
			var method = this.SourceDefinition.Type("Terraria.WindowsLaunch").Method("Main");

			//Get the il processor so we can alter il
			var processor = method.Body.GetILProcessor();

			//Find the instruction that calls the SetConsolCtrlHandler method,
			//and store it as out target instruction
			var target = method.Body.Instructions.Single(x => 
				x.OpCode == OpCodes.Call 
				&& (x.Operand as MethodReference).Name == "SetConsoleCtrlHandler"
			);

			//Remove the two previous instructions
			//		ldc.i4.1
			//		ldsfld _handleRoutine
			processor.Remove(target.Previous);
			processor.Remove(target.Previous);

			//Remove the pop instruction (that removes the result from
			//SetConsoleCtrlHandler from the stack)
			processor.Remove(target.Next);

			//Remove the target instruction now that its not required anymore
			processor.Remove(target);
		}
	}
}
