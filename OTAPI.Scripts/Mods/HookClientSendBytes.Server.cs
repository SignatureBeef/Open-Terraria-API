/*
Copyright (C) 2020 DeathCradle

This file is part of Open Terraria API v3 (OTAPI)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/
#pragma warning disable CS8321 // Local function is declared but never used
#pragma warning disable CS0436 // Type conflicts with imported type

using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;
using MonoMod.Cil;
using System;
using System.Linq;

/// <summary>
/// @doc Creates Hooks.NetMessage.SendBytes. Allows plugins to intercept the sending of data.
/// </summary>
[Modification(ModType.PreMerge, "Hooking Terraria.NetMessage.SendData")]
[MonoMod.MonoModIgnore]
void HookClientSendBytes(MonoModder modder)
{
#if TerrariaServer_SendDataNumber8
    var SendData = modder.GetILCursor(() => Terraria.NetMessage.SendData(default, default, default, default, default, default, default, default, default, default, default, default));
#else
    var SendData = modder.GetILCursor(() => Terraria.NetMessage.SendData(default, default, default, default, default, default, default, default, default, default, default));
#endif

    while (SendData.TryGotoNext(
        i => i.OpCode == OpCodes.Callvirt
            && i.Operand is MethodReference methodReference
            && methodReference.Name == "AsyncSend"
    ))
    {
        SendData.FindNext(out ILCursor[] cursors, i => i.OpCode == OpCodes.Pop && (i.Previous?.OpCode == OpCodes.Leave || i.Previous?.OpCode == OpCodes.Leave_S));

        if (cursors.Length != 1) throw new System.Exception($"Expected to be within a try/catch block");

        var handler = SendData.Method.Body.ExceptionHandlers.Single(x => x.TryEnd == cursors[0].Next);

        if (handler.TryStart.Next.OpCode == OpCodes.Ldloc_S || handler.TryStart.Next.OpCode == OpCodes.Ldarg_1)
        {
            SendData.Emit(handler.TryStart.Next.OpCode, handler.TryStart.Next.Operand);
            SendData.EmitDelegate(OTAPI.Hooks.NetMessage.InvokeSendBytes);
            SendData.Remove();
        }
    }
}

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class NetMessage
        {
            public class SendBytesEventArgs : EventArgs
            {
                public HookResult? Result { get; set; }

                public Terraria.Net.Sockets.ISocket Socket { get; set; }
                public int RemoteClient { get; set; }
                public byte[] Data { get; set; }
                public int Offset { get; set; }
                public int Size { get; set; }
                public global::Terraria.Net.Sockets.SocketSendCallback Callback { get; set; }
                public object State { get; set; }
            }
            public static event EventHandler<SendBytesEventArgs> SendBytes;

            public static void InvokeSendBytes(Terraria.Net.Sockets.ISocket socket, byte[] data, int offset, int size, global::Terraria.Net.Sockets.SocketSendCallback callback, object state, int remoteClient)
            {
                var args = new SendBytesEventArgs()
                {
                    Socket = socket,
                    Data = data,
                    Offset = offset,
                    Size = size,
                    Callback = callback,
                    State = state,
                    RemoteClient = remoteClient
                };
                SendBytes?.Invoke(null, args);
                if (args.Result != HookResult.Cancel)
                {
                    socket.AsyncSend(data, offset, size, callback, state);
                }
            }
        }
    }
}
