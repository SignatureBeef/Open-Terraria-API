using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ModFramework
{
    public static class StackCounter
    {
        public static List<ILCount> Count(MethodDefinition method)
        {
            var instructions = new List<ILCount>();

            var count = 0;
            for (var i = 0; i < method.Body.Instructions.Count; i++)
            {
                var instruction = method.Body.Instructions[i];

                var before = count;
                var pop = instruction.OpCode.StackBehaviourPop;
                var push = instruction.OpCode.StackBehaviourPush;

                if (pop.In(StackBehaviour.PopAll, StackBehaviour.Varpop, StackBehaviour.Varpush))
                    count = 0;
                else
                    count += GetStackBehaviourElements(pop);

                count += GetStackBehaviourElements(push);

                var data = new ILCount()
                {
                    Ins = instruction,
                    OnStackBefore = before,
                    OnStackAfter = count
                };
                instructions.Add(data);

                if (i > 0)
                {
                    instructions[i - 1].Next = data;
                    data.Previous = instructions[i - 1];
                }
            }

            return instructions;
        }

        public static int GetStackBehaviourElements(StackBehaviour stackBehaviour)
        {
            switch (stackBehaviour)
            {
                case StackBehaviour.Pop1:
                case StackBehaviour.Popi:
                case StackBehaviour.Popref:
                    return -1;

                case StackBehaviour.Pop1_pop1:
                case StackBehaviour.Popi_pop1:
                case StackBehaviour.Popi_popi:
                case StackBehaviour.Popi_popi8:
                case StackBehaviour.Popi_popr4:
                case StackBehaviour.Popi_popr8:
                case StackBehaviour.Popref_pop1:
                case StackBehaviour.Popref_popi:
                    return -2;

                case StackBehaviour.Popi_popi_popi:
                case StackBehaviour.Popref_popi_popi:
                case StackBehaviour.Popref_popi_popi8:
                case StackBehaviour.Popref_popi_popr4:
                case StackBehaviour.Popref_popi_popr8:
                case StackBehaviour.Popref_popi_popref:
                    return -3;

                case StackBehaviour.PopAll:
                    throw new NotImplementedException("Unable to determine stack count to offset");

                // calls, rets (consumes stack)
                case StackBehaviour.Varpop:
                    throw new NotImplementedException("Unable to determine call stack count to offset");

                case StackBehaviour.Push1:
                case StackBehaviour.Pushi:
                case StackBehaviour.Pushi8:
                case StackBehaviour.Pushr4:
                case StackBehaviour.Pushr8:
                case StackBehaviour.Pushref:
                    return 1;

                case StackBehaviour.Push1_push1:
                    return 2;

                case StackBehaviour.Pop0:
                case StackBehaviour.Push0:
                case StackBehaviour.Varpush:
                default:
                    return 0;
            }
        }
    }

    public class ILCount
    {
        public Instruction Ins { get; set; }
        public int OnStackBefore { get; set; }
        public int OnStackAfter { get; set; }
        public ILCount Previous { get; set; }
        public ILCount Next { get; set; }

        public ILCount FindPrevious(Func<ILCount, bool> condition)
        {
            var offset = this.Previous;
            while (!condition(offset) && offset != null)
            {
                offset = offset.Previous;
            }

            if (offset == null)
                throw new Exception("Condition was not found");

            return offset;
        }

        public override string ToString()
            => $"({OnStackBefore}=>{OnStackAfter}) {Ins}";
    }
}
