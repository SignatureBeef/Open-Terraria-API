using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Net.Socket
{
	/// <summary>
	/// In this modification we will replace the "new TcpSocket" call in ServerLoop
	/// to allow a custom implementations.
	/// </summary>
	public class ServerSocketCreate : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.4.1, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Netplay.ServerLoop\\TcpSocket...";

		public override void Run()
		{
			var vanilla = SourceDefinition.Type("Terraria.Netplay").Method("ServerLoop");
			var callback = this.Method(() => OTAPI.Callbacks.Terraria.Netplay.ServerSocketCreate());

			var iTcpSocket = vanilla.Body.Instructions.Single(x => x.OpCode == OpCodes.Newobj
												&& x.Operand is MethodReference
												&& (x.Operand as MethodReference).Name == ".ctor"
												&& (x.Operand as MethodReference).DeclaringType.Name == "TcpSocket"
			);

			iTcpSocket.OpCode = OpCodes.Call; //Replace newobj to be call as we need to execute out callback instead
			iTcpSocket.Operand = vanilla.Module.Import(callback); //Replace the method reference from the TcpSocket constructor, to our callback
		}
	}
}