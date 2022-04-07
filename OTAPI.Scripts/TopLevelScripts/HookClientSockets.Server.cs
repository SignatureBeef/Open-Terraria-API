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

#if tModLoaderServer_V1_3
System.Console.WriteLine("Tcp interfaces not available in TML1.3");
#else
using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;
using System;
using Terraria.Net.Sockets;

/// <summary>
/// @doc Creates Hooks.Netplay.CreateTcpListener. Allows custom TCP implementations to be used.
/// </summary>
[Modification(ModType.PreMerge, "Hooking tcp interfaces")]
[MonoMod.MonoModIgnore]
void HookClientSockets(ModFwModder modder)
{
    var callback = modder.GetMethodDefinition(() => OTAPI.Hooks.Netplay.InvokeCreateTcpListener());
    var tcpSocket = modder.GetDefinition<TcpSocket>();
    modder.OnRewritingMethodBody += (MonoModder modder, MethodBody body, Instruction instr, int instri) =>
    {
        if (instr.OpCode == OpCodes.Newobj
            && instr.Operand is MethodReference mref
            && mref.DeclaringType.FullName == tcpSocket.FullName
            && body.Method.DeclaringType.FullName != tcpSocket.FullName
            && body.Method.Name != nameof(OTAPI.Hooks.Netplay.InvokeCreateTcpListener)
        )
        {
            if (mref.Parameters.Count != 0)
                throw new Exception($"Expected no parameters for {tcpSocket.FullName} in {body.Method.FullName}");

            instr.OpCode = OpCodes.Call;
            instr.Operand = callback;
        }
    };
}

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Netplay
        {
            public class CreateTcpListenerEventArgs : EventArgs
            {
                public ISocket Result { get; set; }
            }
            public static event EventHandler<CreateTcpListenerEventArgs> CreateTcpListener;

            public static ISocket InvokeCreateTcpListener()
            {
                var args = new CreateTcpListenerEventArgs();
                CreateTcpListener?.Invoke(null, args);
                return args.Result ?? new TcpSocket();
            }
        }
    }
}

#endif