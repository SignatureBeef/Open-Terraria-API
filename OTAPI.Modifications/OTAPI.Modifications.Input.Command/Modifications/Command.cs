using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Extensions.ILProcessor;
using OTAPI.Patcher.Engine.Modification;
using System;
using System.Linq;

namespace OTAPI.Modifications.Main.Command.Modifications
{
	/// <summary>
	/// This modification will add a command hook to the vanilla command thread.
	/// Typically consumers will just run their own command thread, but this one allows
	/// me to keep vanilla functionality in my test app.
	/// </summary>
	public class Command : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.3.1, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking console commands...";

		public override void Run()
		{
			//Get the method references. One for vanilla, and imported version of our callback
			var mStartDedInputCallBack = this.SourceDefinition.Type("Terraria.Main").Method("startDedInputCallBack");
			var mCallback = this.SourceDefinition.MainModule.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.Main.ProcessCommand(null, null))
			);

			//Get the IL processor instance so we can modify il in the vanilla code
			var processor = mStartDedInputCallBack.Body.GetILProcessor();

			//Find the first two string variables, one is the raw text, the other is lowered
			var vText = mStartDedInputCallBack.Body.Variables[0];
			var vTextLowered = mStartDedInputCallBack.Body.Variables[1];

			if (vText.VariableType != this.SourceDefinition.MainModule.TypeSystem.String)
				throw new NotSupportedException("Expected the first variable to be string");
			if (vTextLowered.VariableType != this.SourceDefinition.MainModule.TypeSystem.String)
				throw new NotSupportedException("Expected the second variable to be string");

			//We want to inject into the first try block, before any other command has been processed.
			//Instead of returning if the callback cancels, we also need to find the end of the try block
			//so we can do a leave instruction instead of a ret (ie continue instead of a return).

			//Find the try block, there will be a "help" comparison 2 instructions in
			var exceptionHandler = mStartDedInputCallBack.Body.ExceptionHandlers.Single(
				x => x.TryStart.Next.OpCode == OpCodes.Ldstr
					&& x.TryStart.Next.Operand.Equals("help")
			);

			//Find the instruction to 'continue' to, ie the last instruction in the try
			var insEndOfTry = exceptionHandler.TryEnd.Previous;
			var insContinue = exceptionHandler.TryStart;

			//Now we can insert our callback at the start of the try block.
			//We dont add all our IL as we need to resume to the original try's start instruction.
			//The ReplaceTransfer call would ruin this for us, so we need to break into 3 parts.
			//Part 1: Insert the first instruction that we intend to be the initial instruction
			//for the try
			var target = exceptionHandler.TryStart.Previous;
			var newStart = processor.InsertAfter(target,
				new { OpCodes.Ldloc, vText }
			).First();

			//Part 2: Swap the current try & other instruction references over to our new receiver
			exceptionHandler.TryStart.ReplaceTransfer(newStart, mStartDedInputCallBack);

			//Part 3: Finish injecting our callback
			processor.InsertAfter(newStart,
				new { OpCodes.Ldloc, vTextLowered },
				new { OpCodes.Call, mCallback },
				new { OpCodes.Brtrue, insContinue },
				new { OpCodes.Leave, insEndOfTry }
			);
		}
	}
}
