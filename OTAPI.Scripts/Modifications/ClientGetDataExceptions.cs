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
using System;
using System.Linq;
using ModFramework;
using Mono.Cecil.Cil;

[Modification(ModType.PreMerge, "Allowing GetData exceptions debugging")]
void ClientGetDataExceptions(ModFramework.ModFwModder modder)
{
    var vanilla = modder.GetMethodDefinition(() => Terraria.NetMessage.CheckBytes(0));

    var handler = vanilla.Body.ExceptionHandlers.Single(x => x.HandlerType == ExceptionHandlerType.Catch);

    var exType = modder.Module.ImportReference(
        typeof(Exception)
    );
    var exVariable = new VariableDefinition(exType);

    vanilla.Body.Variables.Add(exVariable);

    handler.CatchType = modder.Module.ImportReference(
        typeof(Exception)
    );

    handler.HandlerStart.OpCode = OpCodes.Stloc;
    handler.HandlerStart.Operand = exVariable;
    //Console.WriteLine(handler.CatchType);

    var processor = vanilla.Body.GetILProcessor();
    processor.InsertBefore(handler.HandlerEnd.Previous(x => x.OpCode == OpCodes.Leave_S),
        new { OpCodes.Ldloc, exVariable },
        new
        {
            OpCodes.Call,
            Operand = modder.Module.ImportReference(
            typeof(System.Console).GetMethods().Single(x => x.Name == "WriteLine"
                && x.GetParameters().Count() == 1
                && x.GetParameters()[0].ParameterType.Name == "Object"
            )
        )
        }
    );
}