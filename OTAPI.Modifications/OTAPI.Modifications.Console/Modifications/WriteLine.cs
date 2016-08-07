using Mono.Cecil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Patches
{
    public class ConsoleWrites : ModificationBase
    {
        public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
        {
            "TerrariaServer, Version=1.3.2.1, Culture=neutral, PublicKeyToken=null"
        };
        public override string Description => "Hooking all Console.Write/Line calls...";

        public override void Run()
        {
            var cbWrite = SourceDefinition.MainModule.Import(
                Method(() => OTAPI.Callbacks.Terraria.Console.Write(null))
            );
            var cbWriteLine = SourceDefinition.MainModule.Import(
                Method(() => OTAPI.Callbacks.Terraria.Console.WriteLine(null))
            );

            SourceDefinition.MainModule.ForEachInstruction((method, instruction) =>
            {
                var mth = instruction.Operand as MethodReference;
                if (mth != null && mth.DeclaringType.FullName == "System.Console")
                {
                    if (mth.Name == "Write")
                    {
                        instruction.Operand = cbWrite;
                    }
                    else if (mth.Name == "WriteLine")
                    {
                        instruction.Operand = cbWriteLine;
                    }
                }
            });
        }
    }
}
