using Mono.Cecil;
using Mono.Cecil.Cil;
using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modifications.Helpers;
using System;
using System.Linq;

namespace OTAPI.Patcher.Modifications.Hooks.Net
{
	public class SendBytes : OTAPIModification<OTAPIContext>
	{
		public override string Description => "Hooking NetMessage.SendData\\AsyncSend...";

		public override void Run(OptionSet options)
		{
			var vanilla = this.Context.Terraria.Types.NetMessage.Method("SendData");
			var callback = vanilla.Module.Import(this.Context.OTAPI.Types.NetMessage.Method("SendBytes"));

			//Hooking send bytes should be as simple as replacing each AsyncSend call with
			//the OTAPI callback as well as removing the socket instance and leaving the
			//remoteClient/num variable.
			//TODO: Netplay.Connection.Socket AsyncSend for client

			//Find all calls to AsyncSend
			var asyncSendCalls = vanilla.Body.Instructions.Where(x => x.OpCode == OpCodes.Callvirt
																	&& x.Operand is MethodReference
																	&& (x.Operand as MethodReference).Name == "AsyncSend"
																	&& x.Previous(6).Operand is FieldReference
																	&& (x.Previous(6).Operand as FieldReference).Name == "Clients")
																	.ToArray();

			var il = vanilla.Body.GetILProcessor();

			foreach (var call in asyncSendCalls)
			{
				//Replace the call with our OTAPI callback
				call.OpCode = OpCodes.Call;
				call.Operand = vanilla.Module.Import(callback);

				//Wind back to the first Netplay.Clients (there are two, we want the one before the Socket reference)
				var clients = call.Previous(x => x.OpCode == OpCodes.Ldfld
															&& x.Operand is FieldReference
															&& (x.Operand as FieldReference).Name == "Socket")
								  .Previous(x => x.OpCode == OpCodes.Ldsfld
															&& x.Operand is FieldReference
															&& (x.Operand as FieldReference).Name == "Clients");

				//Remove the Socket call
				if (clients.Next.Next.OpCode != OpCodes.Ldelem_Ref) throw new InvalidOperationException($"{clients.Next.Next.OpCode} was not expected.");
				il.Remove(clients.Next.Next); //ldelem.ref
				if (clients.Next.Next.OpCode != OpCodes.Ldfld) throw new InvalidOperationException($"{clients.Next.Next.OpCode} was not expected.");
				il.Remove(clients.Next.Next); //ldfld

				//Remove the client call
				il.Remove(clients);
			}

			Console.WriteLine("Done");
		}
	}
}
//InjectCallback