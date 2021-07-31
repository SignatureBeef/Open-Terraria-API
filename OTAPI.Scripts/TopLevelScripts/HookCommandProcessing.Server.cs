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
System.Console.WriteLine("Command processing not available in TML1.3");
#else
using ModFramework;
using Mono.Cecil.Cil;
using MonoMod;
using MonoMod.Cil;
using System;
using System.Linq;

/// <summary>
/// @doc Creates Hooks.Main.CommandProcess. Allows plugins to intercept issued commands.
/// </summary>
[Modification(ModType.PreMerge, "Hooking command processing")]
void HookCommandProcessing(MonoModder modder)
{
    var startDedInputCallBack = modder.GetILCursor(() => Terraria.Main.startDedInputCallBack());

    var vText = startDedInputCallBack.Body.Variables[0];
    var vTextLowered = startDedInputCallBack.Body.Variables[1];

    if (vText.VariableType.FullName != modder.Module.TypeSystem.String.FullName)
        throw new NotSupportedException("Expected the first variable to be string");
    if (vTextLowered.VariableType.FullName != modder.Module.TypeSystem.String.FullName)
        throw new NotSupportedException("Expected the second variable to be string");

    var exceptionHandler = startDedInputCallBack.Body.ExceptionHandlers.Single(
        x => x.TryStart.Next.OpCode == OpCodes.Ldstr
            && x.TryStart.Next.Operand.Equals("CLI.Help_Command")
    );

    startDedInputCallBack.Goto(exceptionHandler.TryStart, MoveType.Before);
    startDedInputCallBack.Emit(OpCodes.Ldloc, vText);
    var newStart = startDedInputCallBack.Instrs[startDedInputCallBack.Index - 1];

    exceptionHandler.TryStart.ReplaceTransfer(newStart, startDedInputCallBack.Method);

    startDedInputCallBack.Emit(OpCodes.Ldloc, vTextLowered)
        .EmitDelegate<Func<string, string, bool>>(OTAPI.Hooks.Main.InvokeCommandProcess);
    startDedInputCallBack.Emit(OpCodes.Brfalse, exceptionHandler.TryEnd.Previous);
}

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Main
        {
            public class CommandProcessEventArgs : EventArgs
            {
                public HookResult? Result { get; set; }

                public string Command { get; set; }
                public string Lowered { get; set; }
            }
            public static event EventHandler<CommandProcessEventArgs> CommandProcess;

            public static bool InvokeCommandProcess(string lowered, string raw)
            {
                var args = new CommandProcessEventArgs()
                {
                    Lowered = lowered,
                    Command = raw,
                };
                CommandProcess?.Invoke(null, args);
                return args.Result != HookResult.Cancel;
            }
        }
    }
}


#endif