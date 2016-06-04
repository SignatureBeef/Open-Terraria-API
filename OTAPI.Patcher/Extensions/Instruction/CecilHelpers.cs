using Mono.Cecil.Cil;
using System;

namespace OTAPI.Patcher.Extensions
{
    public static partial class CecilHelpers
    {
        public static Instruction FindPreviousInstruction(this Instruction initial, Func<Instruction, Boolean> predicate)
        {
            while (initial.Previous != null)
            {
                if (predicate(initial)) return initial;
                initial = initial.Previous;
            }

            return null;
        }

        public static Instruction FindNextInstruction(this Instruction initial, Func<Instruction, Boolean> predicate)
        {
            while (initial.Next != null)
            {
                if (predicate(initial)) return initial;
                initial = initial.Next;
            }

            return null;
        }

        public static Instruction Previous(this Instruction initial, int count)
        {
            while (count > 0)
            {
                initial = initial.Previous;
                count--;
            }

            return initial;
        }
    }
}
