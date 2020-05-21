using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;
using Terraria.Net.Sockets;

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
			"TerrariaServer, Version=1.4.0.3, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking TcpSocket creations...";

		public override void Run()
		{
            var netplay = this.Type<Terraria.Netplay>();

            var vanilla = netplay.Method("ServerLoop");
			var callback = this.Method(() => OTAPI.Callbacks.Terraria.Netplay.ServerSocketCreate());
			var tcp_socket = this.Type<TcpSocket>().FullName;

			System.Console.WriteLine();

			vanilla.DeclaringType.Module.ForEachInstruction((meth, ins) =>
			{
				if (ins.OpCode == OpCodes.Newobj)
				{
					var methodRef = ins.Operand as MethodReference;

					if (methodRef != null && methodRef.Name == ".ctor")
					{
						if (methodRef.DeclaringType.FullName == tcp_socket && methodRef.Parameters.Count == 0)
						{
							// replace the instruction with our interception callback
							ins.OpCode = OpCodes.Call;
							ins.Operand = vanilla.Module.Import(callback);
							System.Console.WriteLine($"\t-> Replaced instance in {meth.FullName}");
						}
					}
				}
			});
		}
	}
}