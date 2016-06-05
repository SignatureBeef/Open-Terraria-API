using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;

namespace OTAPI.Patcher.Extensions
{
    public static partial class CecilHelpers
    {
        public static Instruction Previous(this Instruction initial, Func<Instruction, Boolean> predicate)
        {
            while (initial.Previous != null)
            {
                if (predicate(initial)) return initial;
                initial = initial.Previous;
            }

            return null;
        }

        public static Instruction Next(this Instruction initial, Func<Instruction, Boolean> predicate)
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

        public static List<Instruction> Next(this Instruction initial, int count = -1)
        {
            var instructions = new List<Instruction>();
            while (initial.Previous != null && (count == -1 || count > 0))
            {
                initial = initial.Previous;
                count--;

                instructions.Add(initial);
            }

            return instructions;
        }
    }
}
