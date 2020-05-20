
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace OTAPI.Extensions
{
    public struct FindPatternResult
    {
        public Instruction first;
        public Instruction last;

        public FindPatternResult(Instruction first, Instruction last)
        {
            this.first = first;
            this.last = last;
        }
    }

    public static class CecilExtensions
    {

        /// <summary>
        /// Finds the first match of the given OpCode pattern
        /// </summary>
        /// <param name="method">The method to search in</param>
        /// <param name="opcodes">The pattern of opcodes</param>
        /// <returns>The first and last instruction found by the pattern.</returns>
        public static FindPatternResult FindPattern(this MethodBody method, Instruction start = null, params OpCode[] opcodes)
        {
            Instruction first = null, last = null;
            var instructions = start == null ? method.Instructions : method.Instructions.Where(x => x.Offset >= start.Offset);
            foreach (var ins in instructions)
            {
                first = null;
                last = null;
                Instruction match = ins;
                foreach (var opcode in opcodes)
                {
                    if (opcode == match.OpCode || default(OpCode) == opcode)
                    {
                        if (first == null) first = match;
                        last = match;
                        match = match.Next;
                    }
                    else
                    {
                        first = null;
                        last = null;
                        break;
                    }
                }

                if (first != null && last != null)
                    break;
            }

            return new FindPatternResult(first, last);
        }
    }
}
