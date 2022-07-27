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

using System;
using System.Linq;
using ModFramework;
using Mono.Cecil.Cil;

/// <summary>
/// @doc A mod to insert Hooks.NetMessage.CheckBytesException and will default to printing to the console
/// </summary>
[Modification(ModType.PreMerge, "Allowing GetData exceptions debugging")]
[MonoMod.MonoModIgnore]
void ClientGetDataExceptions(ModFramework.ModFwModder modder)
{
    var csr = modder.GetILCursor(() => Terraria.NetMessage.CheckBytes(0));

    var handler = csr.Body.ExceptionHandlers.Single(x => x.HandlerType == ExceptionHandlerType.Catch);

    var exType = modder.Module.ImportReference(
        typeof(Exception)
    );
    var exVariable = new VariableDefinition(exType);

    csr.Body.Variables.Add(exVariable);

    handler.CatchType = modder.Module.ImportReference(
        typeof(Exception)
    );

    handler.HandlerStart.OpCode = OpCodes.Stloc;
    handler.HandlerStart.Operand = exVariable;

    csr.Goto(handler.HandlerEnd.Previous(x => x.OpCode == OpCodes.Leave_S), MonoMod.Cil.MoveType.Before);

    csr.Emit(OpCodes.Ldloc, exVariable);
    csr.EmitDelegate(OTAPI.Hooks.NetMessage.InvokeCheckBytesException);
}

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class NetMessage
        {
            public class CheckBytesExceptionEventArgs : EventArgs
            {
                public HookResult? Result { get; set; }

                public Exception Exception { get; set; }
            }
            public static event EventHandler<CheckBytesExceptionEventArgs> CheckBytesException;

            public static void InvokeCheckBytesException(Exception exception)
            {
                var args = new CheckBytesExceptionEventArgs()
                {
                    Exception = exception,
                };
                CheckBytesException?.Invoke(null, args);

                if (args.Result != HookResult.Cancel)
                {
                    Console.WriteLine(exception);
                }
            }
        }
    }
}