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
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ModFramework
{
    [MonoMod.MonoModIgnore]
    public static class StackCounter
    {
        public static int CountStackInputSize(this MethodReference method)
        {
            int count = 0;
            if (method.HasThis) count++;
            count += method.Parameters.Count;

            return count;
        }

        public static List<ILCount> GetStack(this MethodDefinition method)
        {
            var instructions = new List<ILCount>();

            var count = 0;
            for (var i = 0; i < method.Body.Instructions.Count; i++)
            {
                var instruction = method.Body.Instructions[i];

                var before = count;
                var pop = instruction.OpCode.StackBehaviourPop;
                var push = instruction.OpCode.StackBehaviourPush;

                if (pop.In(StackBehaviour.PopAll, StackBehaviour.Varpush))
                    count = 0;
                else
                    count += instruction.GetStackBehaviourElements(pop);

                count += instruction.GetStackBehaviourElements(push);

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

        public static int GetStackBehaviourElements(this Instruction instruction, StackBehaviour stackBehaviour)
        {
            switch (stackBehaviour)
            {
                case StackBehaviour.Pop1:
                case StackBehaviour.Popi:
                case StackBehaviour.Popref:
                    return -1;

                case StackBehaviour.Varpush:
                    {
                        if (instruction.Operand is MethodReference mref)
                        {
                            var is_void = mref.ReturnType == mref.Module.TypeSystem.Void;
                            return is_void ? 0 : 1;
                        }
                        throw new NotImplementedException("Unable to determine call stack count to offset");
                    }

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

                // calls, rets (consumes some of the stack)
                case StackBehaviour.Varpop:
                    {
                        if (instruction.OpCode == OpCodes.Ret)
                            return 0;
                        else if (instruction.Operand is MethodReference mref)
                        {
                            var inputSize = mref.CountStackInputSize();
                            return -inputSize;
                        }
                        throw new NotImplementedException("Unable to determine call stack count to offset");
                    }

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

        public ILCount FindCallStart()
        {
            //var ctx = FindRoot();

            var method = Ins.Operand as MethodReference;

            if (method is null)
                throw new Exception($"Expected the current operand to be a method reference");

            // calc num stack to consume
            // find that offset in the stack offset

            var count = method.CountStackInputSize();
            //var count = 0;

            //if (method.HasThis) count++;
            //count += method.Parameters.Count;

            var offset = this.OnStackBefore - count;

            if (offset < 0)
            {
                var pop = this.Ins.OpCode.StackBehaviourPop;
                var push = this.Ins.OpCode.StackBehaviourPush;
                var aasdasd1 = this.Ins.GetStackBehaviourElements(pop);
                var aasdasd2 = this.Ins.GetStackBehaviourElements(push);
                throw new InvalidOperationException();
            }

            var callStart = FindPrevious(r => r.OnStackBefore == offset);

            //if (ctx != callStart)
            //{

            //}

            return callStart;
        }

        public ILCount FindRoot() => FindPrevious(c => c.OnStackBefore == 0);

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
