using Mono.Cecil;
using Mono.Cecil.Cil;
using NDesk.Options;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

using System;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Net.Socket
{
    /// <summary>
    /// In this modification we wil replace the "new TcpSocket" call in ServerLoop
    /// to allow custom implementations.
    /// </summary>
    public class ServerSocketCreate : ModificationBase
    {
		public override string Description => "Hooking Netplay.ServerLoop\\TcpSocket...";
		
        public override void Run()
        {
            var vanilla = SourceDefinition.Type("Terraria.Netplay").Method("ServerLoop");
            var callback = ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria").Method("ServerSocketCreate");

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
//InjectCallback