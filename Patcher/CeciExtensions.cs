using System;
using Mono.Cecil.Cil;
using Mono.Cecil;
using System.Linq;
using System.Collections.Generic;

namespace OTA.Patcher
{
    public static class CecilMethodExtensions
    {
        public static IEnumerable<MethodDefinition> MatchMethodByParameters(this TypeDefinition source, 
                                                                            IEnumerable<ParameterDefinition> parameters,
                                                                            string methodNameStartWith = null)
        {
            return source.Methods
                .Where(x => x.HasParameters
                && CompareParameters(x.Parameters, parameters)
                && (methodNameStartWith == null || x.Name.StartsWith(methodNameStartWith)));
        }

        public static IEnumerable<MethodDefinition> MatchInstanceMethodByParameters(this TypeDefinition source, 
                                                                                    string instanceTypeName,
                                                                                    IEnumerable<ParameterDefinition> parameters,
                                                                                    string methodNameStartWith = null)
        {
            return source.Methods
                .Where(x => x.HasParameters
                && CompareParameters(x.Parameters.Where(y => y.ParameterType.FullName != instanceTypeName), parameters)
                && (methodNameStartWith == null || x.Name.StartsWith(methodNameStartWith)));
        }

        /// <summary>
        /// Compares the parameters to see if they expect the same.
        /// </summary>
        /// <returns><c>true</c>, if parameters was compared, <c>false</c> otherwise.</returns>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        internal static bool CompareParameters(IEnumerable<ParameterDefinition> a, IEnumerable<ParameterDefinition> b)
        {
            if (a.Count() == b.Count())
            {

                for (var x = 0; x < a.Count(); x++)
                {
                    if (a.ElementAt(x).ParameterType.FullName != b.ElementAt(x).ParameterType.FullName)
                        return false;
                }
                return true;
            }

            return false;
        }

        public static void ComputeOffsets(this MethodBody body)
        {
            int offset = 0;
            foreach (var current in body.Instructions)
            {
                current.Offset = offset;
                offset += current.GetSize();
            }
        }

        public static void ReplaceTransfer(this Instruction current, Instruction newTarget, MethodDefinition method)
        {
            //If a method has a body then check the instruction targets & exceptions
            if (method.HasBody)
            {
                foreach (var ins in method.Body.Instructions.Where( x=> x.Operand == current))
                    ins.Operand = newTarget;

                if (method.Body.HasExceptionHandlers)
                {
                    foreach (var handler in method.Body.ExceptionHandlers)
                    {
                        if (handler.FilterStart == current) handler.FilterStart = newTarget;
                        if (handler.HandlerEnd == current) handler.HandlerEnd = newTarget;
                        if (handler.HandlerStart == current) handler.HandlerStart = newTarget;
                        if (handler.TryEnd == current) handler.TryEnd = newTarget;
                        if (handler.TryStart == current) handler.TryStart = newTarget;
                    }
                }

                newTarget.Offset = current.Offset;
                newTarget.SequencePoint = current.SequencePoint;
                newTarget.Offset++;
            }
        }

        public static bool IsInWhileLoop(this Instruction position)
        {
            //            position.
            return false;
        }

        public static Instruction Clone(this Instruction initial)
        {
            Instruction ins;

            //Create the clone
            if (initial.Operand == null) ins = Instruction.Create(initial.OpCode);
            else if (initial.Operand is Byte) ins = Instruction.Create(initial.OpCode, (byte)initial.Operand);
            else if (initial.Operand is Int32) ins = Instruction.Create(initial.OpCode, (int)initial.Operand);
            else if (initial.Operand is Int64) ins = Instruction.Create(initial.OpCode, (long)initial.Operand);
            else if (initial.Operand is Single) ins = Instruction.Create(initial.OpCode, (float)initial.Operand);
            else if (initial.Operand is Double) ins = Instruction.Create(initial.OpCode, (double)initial.Operand);
            else if (initial.Operand is Instruction) ins = Instruction.Create(initial.OpCode, (Instruction)initial.Operand);
            else if (initial.Operand is Instruction[]) ins = Instruction.Create(initial.OpCode, (Instruction[])initial.Operand);
            else if (initial.Operand is VariableDefinition) ins = Instruction.Create(initial.OpCode, (VariableDefinition)initial.Operand);
            else if (initial.Operand is SByte) ins = Instruction.Create(initial.OpCode, (SByte)initial.Operand);
            else if (initial.Operand is TypeReference) ins = Instruction.Create(initial.OpCode, (TypeReference)initial.Operand);
            else if (initial.Operand is CallSite) ins = Instruction.Create(initial.OpCode, (CallSite)initial.Operand);
            else if (initial.Operand is MethodReference) ins = Instruction.Create(initial.OpCode, (MethodReference)initial.Operand);
            else if (initial.Operand is String) ins = Instruction.Create(initial.OpCode, (String)initial.Operand);
            else if (initial.Operand is ParameterDefinition) ins = Instruction.Create(initial.OpCode, (ParameterDefinition)initial.Operand);
            else if (initial.Operand is FieldReference) ins = Instruction.Create(initial.OpCode, (FieldReference)initial.Operand);
            else throw new NotSupportedException("Operand");

            //Update other properties
            //            ins.Offset = initial.Offset;
            //            ins.SequencePoint = initial.SequencePoint;

            return ins;
        }

        public static List<Instruction> GetInstructionsUntil(this Instruction initial, OpCode opCode)
        {
            var instructions = new List<Instruction>();
            instructions.Add(initial);
            if (initial.Next != null) initial = initial.Next;

            while (true)
            {
                if (initial.OpCode == opCode)
                {
                    break;
                }
                instructions.Add(initial);

                if (initial.Next == null) break;
                initial = initial.Next;
            }

            return instructions;
        }

        public static Instruction FindInstructionByOpCodeBefore(this Instruction initial, OpCode opCode)
        {
            while (initial.Previous != null)
            {
                if (initial.Previous.OpCode == opCode) return initial.Previous;
                initial = initial.Previous;
            }

            return null;
        }

        /// <summary>
        /// Wraps the method after finding the Begin/End variant callbacks of methodName
        /// </summary>
        /// <param name="method"></param>
        /// <param name="typeDefinition"></param>
        /// <param name="methodName"></param>
        public static void WrapBeginEnd(this MethodDefinition method, TypeDefinition typeDefinition, string methodName, bool beginIsCancellable = false)
        {
            var cbkBegin = typeDefinition.Methods.Single(x => x.Name == methodName + "Begin");
            var cbkEnd = typeDefinition.Methods.Single(x => x.Name == methodName + "End");

            method.Wrap(cbkBegin, cbkEnd, beginIsCancellable);
        }

        /// <summary>
        /// Wraps the method after finding the Begin/End variant callbacks of methodName
        /// </summary>
        /// <param name="method"></param>
        /// <param name="typeDefinition"></param>
        /// <param name="methodName"></param>
        public static void InjectBeginEnd(this MethodDefinition method, TypeDefinition typeDefinition, string methodName, bool beginIsCancellable = false)
        {
            var cbkBegin = typeDefinition.Methods.Single(x => x.Name == methodName + "Begin");
            var cbkEnd = typeDefinition.Methods.Single(x => x.Name == methodName + "End");

            method.Inject(cbkBegin, cbkEnd, beginIsCancellable);
        }

        /// <summary>
        /// Wraps the method with the specified begin/end calls
        /// </summary>
        /// <param name="method"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        public static void Inject(this MethodDefinition method, MethodDefinition begin, MethodDefinition end, bool beginIsCancellable = false)
        {
            if (!method.HasBody) throw new InvalidOperationException("Method must have a body.");
            if (method.ReturnType.Name == "Void")
            {
                //Import the callbacks to the calling methods assembly
                var impBegin = method.Module.Import(begin);
                var impEnd = method.Module.Import(end);

                var xstFirst = method.Body.Instructions.First();
                var xstLast = method.Body.Instructions.Last(x => x.OpCode == OpCodes.Ret);
                var lastInstruction = method.Body.Instructions.Last();

                var il = method.Body.GetILProcessor();

                if (begin.HasParameters) //Parameter order must be the same
                {
                    for (var i = 0; i < begin.Parameters.Count; i++)
                    {
                        var prm = method.CreateParameterInstruction(i);
                        il.InsertBefore(xstFirst, prm);
                    }
                }
                il.InsertBefore(xstFirst, il.Create(OpCodes.Call, impBegin));


                if (end.HasParameters) //Parameter order must be the same
                {
                    for (var i = 0; i < end.Parameters.Count; i++)
                    {
                        var prm = method.CreateParameterInstruction(i);
                        il.InsertBefore(xstLast, prm);
                    }
                }
                il.InsertBefore(xstLast, il.Create(OpCodes.Call, impEnd));
                
//                Instruction insertion = il.Create(OpCodes.Ret);
//                Instruction endpoint = null;
//                il.InsertAfter(lastInstruction, insertion);
//                
//                var nop = il.Create(OpCodes.Nop);
//                if (null == endpoint) endpoint = nop;
//                il.InsertBefore(insertion, nop);
//                
//                if (end.HasParameters) //Parameter order must be the same
//                {
//                    for (var i = 0; i < end.Parameters.Count; i++)
//                    {
//                        var prm = method.CreateParameterInstruction(i);
//                        il.InsertBefore(insertion, prm);
//                        //                        il.InsertBefore(lastInstruction, prm);
//                        if (null == endpoint) endpoint = prm;
//                    }
//                }
//                
//                var injected = new List<Instruction>();
//                var total = 0;
//                while (total++ < 6)
//                {
//                    var ins = method.Body.Instructions.Except(injected).First(xx => xx.OpCode == OpCodes.Ret);
//                
//                    if (end.HasParameters) //Parameter order must be the same
//                    {
//                        for (var i = 0; i < end.Parameters.Count; i++)
//                        {
//                            var prm = method.CreateParameterInstruction(i);
//                            il.InsertBefore(ins, prm);
//                        }
//                    }
//                    il.InsertBefore(ins, il.Create(OpCodes.Call, impEnd));
//                    il.InsertBefore(ins, il.Create(OpCodes.Pop));
//                
//                    injected.Add(ins);
//                }
//                
//                //Since we are instructed to expect a cancellation, a hardcoded false is expected. If false then return the default.
//                if (beginIsCancellable)
//                {
//                
//                }
            }
            else throw new NotSupportedException("Non Void methods not yet supported");
        }

        /// <summary>
        /// Wraps the method with the specified begin/end calls
        /// </summary>
        /// <param name="method"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        public static void Wrap(this MethodDefinition method, MethodDefinition begin, MethodDefinition end, bool beginIsCancellable = false)
        {
            if (!method.HasBody) throw new InvalidOperationException("Method must have a body.");
            if (method.ReturnType.Name == "Void")
            {
                //Import the callbacks to the calling methods assembly
                var impBegin = method.Module.Import(begin);
                var impEnd = method.Module.Import(end);

                var instanceMethod = (method.Attributes & MethodAttributes.Static) == 0;

                //Create the new replacement method
                var wrapped = new MethodDefinition(method.Name, MethodAttributes.Public, method.ReturnType);
                //Rename the vanilla
                method.Name = method.Name + "_wrapped";

                //Copy over parameters
                if (method.HasParameters)
                    foreach (var prm in method.Parameters)
                    {
                        wrapped.Parameters.Add(prm);
                    }

                //Place the new method in the declaring type of the method we are cloning
                method.DeclaringType.Methods.Add(wrapped);

                //Now, create the IL body
                var il = wrapped.Body.GetILProcessor();

                //Execute the begin hook
                if (instanceMethod)
                    il.Emit(OpCodes.Ldarg_0);
                if (method.HasParameters)
                    for (var i = 0; i < method.Parameters.Count; i++)
                        il.Emit(OpCodes.Ldarg, method.Parameters[i]);
                il.Emit(OpCodes.Call, impBegin);

                //If the begin call has a value, pop it for the time being
                if (impBegin.ReturnType.Name != "Void")
                    il.Emit(OpCodes.Pop);

                //Execute the actual code
                //If the call is an instance method, then ensure the Ldarg_0 is emitted.
                if (instanceMethod)
                    il.Emit(OpCodes.Ldarg_0);
                if (method.HasParameters)
                    for (var i = 0; i < method.Parameters.Count; i++)
                        il.Emit(OpCodes.Ldarg, method.Parameters[i]);
                il.Emit(OpCodes.Call, method);

                //TODO pass the return value onto the end hook
                if (method.ReturnType.Name != "Void")
                    il.Emit(OpCodes.Pop);
                
                //Execute the end hook
                if (instanceMethod)
                    il.Emit(OpCodes.Ldarg_0);
                if (method.HasParameters)
                    for (var i = 0; i < method.Parameters.Count; i++)
                        il.Emit(OpCodes.Ldarg, method.Parameters[i]);
                il.Emit(OpCodes.Call, impEnd);

                //If the end call has a value, pop it for the time being
                if (impEnd.ReturnType.Name != "Void")
                    il.Emit(OpCodes.Pop);

                //Exit the method
                il.Emit(OpCodes.Ret);



                //                var xstFirst = method.Body.Instructions.First();
                ////                var xstLast = method.Body.Instructions.Last(x => x.OpCode == OpCodes.Ret);
                //                var lastInstruction = method.Body.Instructions.Last();
                //
                //                if (begin.HasParameters) //Parameter order must be the same
                //                {
                //                    for (var i = 0; i < begin.Parameters.Count; i++)
                //                    {
                //                        var prm = method.CreateParameterInstruction(i);
                //                        il.InsertBefore(xstFirst, prm);
                //                    }
                //                }
                //                il.InsertBefore(xstFirst, il.Create(OpCodes.Call, impBegin));
                //
                //
                //
                //                Instruction insertion = il.Create(OpCodes.Ret);
                //                Instruction endpoint = null;
                //                il.InsertAfter(lastInstruction, insertion);
                //
                //
                //
                //                var nop = il.Create(OpCodes.Nop);
                //                if (null == endpoint) endpoint = nop;
                //                il.InsertBefore(insertion, nop);
                //
                //                if (end.HasParameters) //Parameter order must be the same
                //                {
                //                    for (var i = 0; i < end.Parameters.Count; i++)
                //                    {
                //                        var prm = method.CreateParameterInstruction(i);
                //                        il.InsertBefore(insertion, prm);
                ////                        il.InsertBefore(lastInstruction, prm);
                //                        if (null == endpoint) endpoint = prm;
                //                    }
                //                }
                //
                //                var injected = new List<Instruction>();
                //                var total = 0;
                //                while (total++ < 6)
                //                {
                //                    var ins = method.Body.Instructions.Except(injected).First(xx => xx.OpCode == OpCodes.Ret);
                //
                //                    if (end.HasParameters) //Parameter order must be the same
                //                    {
                //                        for (var i = 0; i < end.Parameters.Count; i++)
                //                        {
                //                            var prm = method.CreateParameterInstruction(i);
                //                            il.InsertBefore(ins, prm);
                //                        }
                //                    }
                //                    il.InsertBefore(ins, il.Create(OpCodes.Call, impEnd));
                //                    il.InsertBefore(ins, il.Create(OpCodes.Pop));
                //
                //                    injected.Add(ins);
                //                }
                //
                //                //Since we are instructed to expect a cancellation, a hardcoded false is expected. If false then return the default.
                //                if (beginIsCancellable)
                //                {
                //
                //                }
            }
            else throw new NotSupportedException("Non Void methods not yet supported");
        }

        public static Instruction CreateParameterInstruction(this MethodDefinition method, int index)
        {
            return Instruction.Create(OpCodes.Ldarg, method.Parameters[index]);
        }

        public static void EmptyInstructions(this MethodDefinition method)
        {
            if (method.HasBody)
            {
                //Clear out everything
                method.Body.Variables.Clear();
                method.Body.ExceptionHandlers.Clear();
                method.Body.Instructions.Clear();

                //Now it needs the return instruction.
                var il = method.Body.GetILProcessor();
                if (method.ReturnType.IsValueType)
                {
                    //return default(<return type>);
                    method.Body.Instructions.Insert(0, il.Create(OpCodes.Ret));

                    if (method.ReturnType.IsPrimitive)
                    {
                        //If a primitive type then pushing this onto the stack will trigger, for example default(bool)
                        method.Body.Instructions.Insert(0, il.Create(OpCodes.Ldc_I4_0));
                    }
                    else
                    {
                        //This is not yet tested. But it should, say for example, struct testing {}, create default(testing)

                        //What needs to happen is we need to create a local variable to store the [initobj], then we can safely return it
                        //It should len decompile to return default(xyz)

                        ///Create the variable - this one goes at index 0
                        VariableDefinition vr1, vr2;
                        method.Body.Variables.Add(vr1 = new VariableDefinition(method.ReturnType));
                        method.Body.Variables.Add(vr2 = new VariableDefinition(method.ReturnType));

                        ////Initialise the variable
                        method.Body.Instructions.Clear();
                        method.Body.Instructions.Add(il.Create(OpCodes.Nop));
                        method.Body.Instructions.Add(il.Create(OpCodes.Ldloca_S, vr1));
                        method.Body.Instructions.Add(il.Create(OpCodes.Initobj, method.ReturnType));
                        method.Body.Instructions.Add(il.Create(OpCodes.Ldloc_0));
                        method.Body.Instructions.Add(il.Create(OpCodes.Stloc_1));

                        var ins = il.Create(OpCodes.Ldloc_1);
                        method.Body.Instructions.Add(il.Create(OpCodes.Br_S, ins));

                        method.Body.Instructions.Add(ins);
                        method.Body.Instructions.Add(il.Create(OpCodes.Ret));

                        //Return it
                    }
                }
                else if (method.ReturnType.MetadataType == MetadataType.Void)
                {
                    //Behind the scenes there is a return opcode
                    method.Body.Instructions.Insert(0, il.Create(OpCodes.Ret));
                }
                else
                {
                    //return null;
                    method.Body.Instructions.Insert(0, il.Create(OpCodes.Ret));
                    method.Body.Instructions.Insert(0, il.Create(OpCodes.Ldnull));
                }
            }
        }
    }
}

