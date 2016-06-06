using Mono.Cecil;
using System;
using System.Linq;

namespace OTAPI.Patcher.Extensions
{
    public static partial class CecilHelpers
    {
        public static TypeDefinition Type(this ModuleDefinition moduleDefinition, string name)
        {
            return moduleDefinition.Types.Single(x => x.FullName == name);
        }

        /// <summary>
        /// Enumerates all instructions in all methods across each type of the assembly
        /// </summary>
        /// <param name="module"></param>
        /// <param name="callback"></param>
        public static void ForEachInstruction(this ModuleDefinition module, Action<MethodDefinition, Mono.Cecil.Cil.Instruction> callback)
        {
            foreach (var type in module.Types)
            {
                foreach (var mth in type.Methods)
                {
                    if (mth.HasBody)
                    {
                        foreach (var ins in mth.Body.Instructions.ToArray())
                            callback.Invoke(mth, ins);
                    }
                }
            }
        }
    }
}
