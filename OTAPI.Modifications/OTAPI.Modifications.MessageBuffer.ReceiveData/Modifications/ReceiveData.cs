using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Net
{
	public class ReceiveData : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.4.3, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking NetMessage.GetData";

		public override void Run()
		{
			int tmp = 0;
			byte tmpB = 0;
			var vanilla = this.Method(() => (new Terraria.MessageBuffer()).GetData(0, 0, out tmp));
			var callback = this.Method(() => OTAPI.Callbacks.Terraria.MessageBuffer.ReceiveData(null, ref tmpB, ref tmp, ref tmp, ref tmp, ref tmp));

			//In this episode we are injecting the hook in a place where it can modify the packet id
			//as soon as it can. The target instruction will be the just after the packet id byte is
			//read from the buffer, and before the mac packet id check. Luckily the variable "messageType"
			//is being set right between these conditions, so we can replace the value on the stack
			//with the result value from the call to our callback!

			//Note to self: the above is done without consideration to max packets. max packets should
			//              be a separate patch that alters the value in the condition. ideally the mod
			//              running OTAPI should have a packet register (OTAPI 1 provided this).


			//Time to find the messageType variable
			//It's the first ldloc.0 after read buffer
			var target = vanilla.Body.Instructions.First(x => x.OpCode == OpCodes.Ldloc_0
				//After read buffer
				&& x.Offset > vanilla.Body.Instructions.First(y => y.OpCode == OpCodes.Ldfld
																&& y.Operand is FieldReference
																&& (y.Operand as FieldReference).Name == "readBuffer"
															 ).Offset
			);

			//Change the target to our callback, which will pop it's return value on the stack
			target.OpCode = OpCodes.Call;
			target.Operand = vanilla.Module.Import(callback);

			var il = vanilla.Body.GetILProcessor();

			//We now need to add arguments to our callback, by reference so we can alter them.

			//Manually add the current instance of MessageBuffer
			il.InsertBefore(target, il.Create(OpCodes.Ldarg_0));

			//Manually add the packet id and read offset
			il.InsertBefore(target, il.Create(OpCodes.Ldloca, vanilla.Body.Variables.First(x => x.VariableType.Name == "Byte"))); //First byte is b
			il.InsertBefore(target, il.Create(OpCodes.Ldloca, vanilla.Body.Variables.First(x => x.VariableType.Name == "Int32"))); //First int is read offset

			//Add parameters by reference
			foreach (var prm in vanilla.Parameters)
			{
				il.InsertBefore(target, il.Create(OpCodes.Ldarga, prm));
			}
		}
	}
}
//InjectCallback