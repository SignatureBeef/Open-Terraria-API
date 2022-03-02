using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

using System;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Net
{
	public class SendBytes : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.3.6, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking NetMessage.SendData\\AsyncSend...";

		public override void Run()
		{
			var vanilla = this.Method(() => Terraria.NetMessage.SendData(0, -1, -1, Terraria.Localization.NetworkText.Empty, 0, 0, 0, 0, 0, 0, 0));
			var callback = vanilla.Module.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.NetMessage.SendBytes(0, null, 0, 0, null, null))
			);

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
				call.Operand = callback;

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
		}
	}
}
