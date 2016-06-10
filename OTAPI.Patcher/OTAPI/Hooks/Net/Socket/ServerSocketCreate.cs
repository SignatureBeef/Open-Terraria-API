using Mono.Cecil;
using Mono.Cecil.Cil;
using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modifications.Helpers;
using System;
using System.Linq;

namespace OTAPI.Patcher.Modifications.Hooks.Net.Socket
{
    /// <summary>
    /// In this modification we wil replace the "new TcpSocket" call in ServerLoop
    /// to allow custom implementations.
    /// </summary>
    public class ServerSocketCreate : OTAPIModification<OTAPIContext>
    {
        public override void Run(OptionSet options)
        {
            Console.Write("Hooking Netplay.ServerLoop\\TcpSocket...");

            var vanilla = this.Context.Terraria.Types.Netplay.Method("ServerLoop");
            var callback = this.Context.OTAPI.Types.Netplay.Method("ServerSocketCreate");

            var iTcpSocket = vanilla.Body.Instructions.Single(x => x.OpCode == OpCodes.Newobj
                                                && x.Operand is MethodReference
                                                && (x.Operand as MethodReference).Name == ".ctor"
                                                && (x.Operand as MethodReference).DeclaringType.Name == "TcpSocket"
            );

            iTcpSocket.OpCode = OpCodes.Call; //Replace newobj to be call as we need to execute out callback instead
            iTcpSocket.Operand = vanilla.Module.Import(callback); //Replace the method reference from the TcpSocket constructor, to our callback

            Console.WriteLine("Done");
        }
    }
}
//InjectCallback