using System;
using System.Linq;
using ModFramework;
using Mono.Cecil.Cil;

[Modification(ModType.PreMerge, "ALlowing GetData exceptions debugging")]
void ClientGetDataExceptions(ModFramework.ModFwModder modder)
{
    var vanilla = modder.GetMethodDefinition(() => Terraria.NetMessage.CheckBytes(0), followRedirect: true);

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