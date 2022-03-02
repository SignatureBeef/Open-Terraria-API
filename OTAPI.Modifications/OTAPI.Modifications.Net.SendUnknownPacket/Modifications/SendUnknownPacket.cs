using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Extensions.ILProcessor;
using OTAPI.Patcher.Engine.Modification;
using System;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Net
{
	/// <summary>
	/// In this modification we add the "default" statement to the SendData switch statement.
	/// This is so I can also add a hook to allow custom packets to be sent so we don't have to
	/// replicate SendData code.
	/// </summary>
	[Ordered(4)]
	public class SendUnknownPacket : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.3.6, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking NetMessage.SendData...";

		public override void Run()
		{
			//Get the vanilla callback, as well as the imported OTAPI callback method
			var vanilla = this.Method(() => Terraria.NetMessage.SendData(0, -1, -1, Terraria.Localization.NetworkText.Empty, 0, 0, 0, 0, 0, 0, 0));
			var callback = vanilla.Module.Import(this.Method(() =>
				OTAPI.Callbacks.Terraria.NetMessage.SendUnknownPacket(0, null, 0, 0, 0, Terraria.Localization.NetworkText.Empty, 0, 0, 0, 0, 0, 0, 0)
			));

			//Get the IL processor instance so we can alter IL
			var il = vanilla.Body.GetILProcessor();

			//Get the buffer id and writer variable references
			var vrbBufferId = vanilla.Body.Variables[0];
			var vrbWriter = vanilla.Body.Variables[3];

			//Ensure the data types are correct
			if (vrbBufferId.VariableType != vanilla.Module.TypeSystem.Int32)
				throw new NotSupportedException("Expected the first variable to be the buffer id");
			if (vrbWriter.VariableType.Name != "BinaryWriter")
				throw new NotSupportedException("Expected the second variable to be the writer");

			//Find the switch statement we wish to add the "default" block to
			var insSwitchStatement = vanilla.Body.Instructions.Single(x => x.OpCode == OpCodes.Switch);
			//Find the instruction that is called when the switch completes
			var insInstructionAfterSwitch = insSwitchStatement.Next.Operand as Instruction;

			//Insert the jump when the last switch block ends, otherwise it will continue onto
			//our custom code we will soon inject.
			il.InsertBefore(insInstructionAfterSwitch,
				new { OpCodes.Br_S, insInstructionAfterSwitch }
			);

			//Start building the custom code by supplying the bufferId and writer variables
			var instructions = il.InsertBefore(insInstructionAfterSwitch,
				new { OpCodes.Ldloc, Operand = vrbBufferId },
				new { OpCodes.Ldloc, Operand = vrbWriter }
			);

			//Add all SendData parameters to our callback (as we also need these for the hooks)
			foreach (var parameter in vanilla.Parameters)
				il.InsertBefore(insInstructionAfterSwitch, il.Create(OpCodes.Ldarg, parameter));

			//Finish up injecting the custom IL.
			il.InsertBefore(insInstructionAfterSwitch,
				//Execute the callback with whats on the stack
				new { OpCodes.Call, Operand = callback }
			);

			//Instead of continuing onto the [insInstructionAfterSwitch] we change this to
			//our newly injected IL, which will trigger the 'default' keyword.
			if (insSwitchStatement.Next.Operand != insInstructionAfterSwitch)
				throw new NotSupportedException("Expected default switch jump to be the end");
			insSwitchStatement.Next.Operand = instructions.First();
			insSwitchStatement.Next.OpCode = OpCodes.Br;

			//Cleanup & optimize
			vanilla.Body.SimplifyMacros();
			vanilla.Body.OptimizeMacros();
		}
	}
}
