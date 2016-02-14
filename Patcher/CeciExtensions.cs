using System;
using Mono.Cecil.Cil;
using Mono.Cecil;
using System.Linq;
using System.Collections.Generic;

namespace OTA.Patcher
{
    public static class CecilMethodExtensions
    {
        const String WrappedMethodNameSuffix = "Direct";

        public static MethodDefinition Method(this TypeDefinition typeDefinition, string name)
        {
            return typeDefinition.Methods.Single(x => x.Name == name);
        }

        public static FieldDefinition Field(this TypeDefinition typeDefinition, string name)
        {
            return typeDefinition.Fields.Single(x => x.Name == name);
        }

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

        public static void ReplaceWith(this MethodDefinition method, MethodDefinition replacement)
        {
            foreach (var type in method.Module.Types)
            {
                foreach (var mth in type.Methods)
                {
                    if (mth.HasBody)
                    {
                        foreach (var ins in mth.Body.Instructions)
                        {
                            if (ins.Operand == method)
                                ins.Operand = replacement;
                        }
                    }
                }
            }
        }

        public static void ReplaceTransfer(this Instruction current, Instruction newTarget, MethodDefinition method)
        {
            //If a method has a body then check the instruction targets & exceptions
            if (method.HasBody)
            {
                foreach (var ins in method.Body.Instructions.Where(x => x.Operand == current))
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

        public static Instruction FindPreviousInstructionByOpCode(this Instruction initial, OpCode opCode)
        {
            while (initial.Previous != null)
            {
                if (initial.Previous.OpCode == opCode) return initial.Previous;
                initial = initial.Previous;
            }

            return null;
        }

        public static Instruction FindNextInstructionByOpCode(this Instruction initial, OpCode opCode)
        {
            while (initial.Next != null)
            {
                if (initial.Next.OpCode == opCode) return initial.Next;
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
                //                var lastInstruction = method.Body.Instructions.Last();

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

        public static Instruction InjectBeginCallback(this MethodDefinition method, MethodReference begin, bool instanceMethod, bool beginIsCancellable = false)
        {
            Instruction targetInstruction = null;

            //Import the callbacks to the calling methods assembly
            var impBegin = method.Module.Import(begin);

            //            var instanceMethod = (method.Attributes & MethodAttributes.Static) == 0;

            //Now, create the IL body
            var il = method.Body.GetILProcessor();

            //Execute the begin hook
            if (instanceMethod)
                il.Emit(OpCodes.Ldarg_0);
            if (method.HasParameters)
                for (var i = 0; i < method.Parameters.Count; i++)
                    il.Emit(OpCodes.Ldarg, method.Parameters[i]);
            il.Emit(OpCodes.Call, impBegin);

            //Create the cancel return if required.
            targetInstruction = null; //Will eventually be transformed to a Brtrue_S in order to transfer to the normal code.
            if (beginIsCancellable)
            {
                targetInstruction = il.Create(OpCodes.Nop);
                il.Append(targetInstruction);

                //Emit the cancel handling
                if (method.ReturnType.Name == "Void")
                    il.Emit(OpCodes.Ret);
                else
                {
                    //Return a default value
                    VariableDefinition vr1;
                    method.Body.Variables.Add(vr1 = new VariableDefinition(method.ReturnType));

                    //Initialise the variable
                    il.Emit(OpCodes.Ldloca_S, vr1);
                    il.Emit(OpCodes.Initobj, method.ReturnType);
                    il.Emit(OpCodes.Ldloc, vr1);

                    il.Emit(OpCodes.Ret);
                }
            }
            else if (impBegin.ReturnType.Name != "Void")
            {
                targetInstruction = il.Create(OpCodes.Pop);
                il.Append(targetInstruction);
            }

            return targetInstruction;
        }

        public static void InjectEndCallback(this MethodDefinition method, MethodReference end, bool instanceMethod)
        {
            //Import the callbacks to the calling methods assembly
            var impEnd = method.Module.Import(end);

            //            var instanceMethod = (method.Attributes & MethodAttributes.Static) == 0;

            //Now, create the IL body
            var il = method.Body.GetILProcessor();

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
        }

        public static void InjectMethodEnd(this MethodDefinition method, bool noHandling = false)
        {
            var il = method.Body.GetILProcessor();
            //Exit the method
            //If the end call has a value, pop it for the time being
            if (false == noHandling && method.ReturnType.Name != "Void")
            {
                VariableDefinition vr1;
                method.Body.Variables.Add(vr1 = new VariableDefinition("cancelDefault", method.ReturnType));

                //Initialise the variable
                il.Emit(OpCodes.Ldloca_S, vr1);
                il.Emit(OpCodes.Initobj, method.ReturnType);
                il.Emit(OpCodes.Ldloc, vr1);
            }
            il.Emit(OpCodes.Ret);
        }

        public static Instruction InjectMethodCall(this MethodDefinition method, MethodReference target, bool instanceMethod, bool emitNonVoidPop = true)
        {
            Instruction firstInstruction = null;
            //            var instanceMethod = (target.Attributes & MethodAttributes.Static) == 0;

            //Now, create the IL body
            var il = method.Body.GetILProcessor();

            //If the call is an instance method, then ensure the Ldarg_0 is emitted.
            if (instanceMethod)
            {
                //Set the instruction to be resumed upon not cancelling, if not already
                var instance = il.Create(OpCodes.Ldarg_0);
                //                if (beginIsCancellable && beginResult != null && beginResult.OpCode == OpCodes.Nop)
                //                {
                //                    beginResult.OpCode = OpCodes.Brtrue_S;
                //                    beginResult.Operand = instance;
                //                }
                if (null == firstInstruction) firstInstruction = instance;

                il.Append(instance);
            }

            //Create parameters - TODO call optimise
            if (target.HasParameters)
                for (var i = 0; i < target.Parameters.Count; i++)
                {
                    var prm = il.Create(OpCodes.Ldarg, target.Parameters[i]);
                    if (null == firstInstruction) firstInstruction = prm;
                    //                    //Set the instruction to be resumed upon not cancelling, if not already
                    //                    if (beginIsCancellable && beginResult != null && beginResult.OpCode == OpCodes.Nop)
                    //                    {
                    //                        beginResult.OpCode = OpCodes.Brtrue_S;
                    //                        beginResult.Operand = prm;
                    //                    }

                    il.Append(prm);
                }

            //Call the begin hook
            var call = il.Create(OpCodes.Call, target);
            //            //Set the instruction to be resumed upon not cancelling, if not already
            //            if (beginIsCancellable && beginResult != null && beginResult.OpCode == OpCodes.Nop)
            //            {
            //                beginResult.OpCode = OpCodes.Brtrue_S;
            //                beginResult.Operand = call;
            //            }
            if (null == firstInstruction) firstInstruction = call;
            il.Append(call);

            //If a value is returned, ensure it's removed from the stack
            if (emitNonVoidPop && target.ReturnType.Name != "Void")
                il.Emit(OpCodes.Pop);

            return firstInstruction;
        }

        /// <summary>
        /// Wraps the method with the specified begin/end calls
        /// </summary>
        /// <param name="method"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        public static MethodDefinition Wrap(this MethodDefinition method, MethodReference begin,
                                            MethodReference end = null,
                                            bool beginIsCancellable = false,
                                            bool noEndHandling = false)
        {
            if (!method.HasBody) throw new InvalidOperationException("Method must have a body.");
            if (method.ReturnType.Name == "Void" || method.ReturnType.Name == "String")
            {
                //Create the new replacement method
                var wrapped = new MethodDefinition(method.Name, method.Attributes, method.ReturnType);
                var instanceMethod = (method.Attributes & MethodAttributes.Static) == 0;

                //Rename the existing method, and replace it
                method.Name = method.Name + WrappedMethodNameSuffix;
                method.ReplaceWith(wrapped);

                //Copy over parameters
                if (method.HasParameters)
                    foreach (var prm in method.Parameters)
                    {
                        wrapped.Parameters.Add(prm);
                    }

                //Place the new method in the declaring type of the method we are cloning
                method.DeclaringType.Methods.Add(wrapped);

                var beginResult = wrapped.InjectBeginCallback(begin, instanceMethod, beginIsCancellable);

                //Execute the actual code
                var insFirstForMethod = wrapped.InjectMethodCall(method, instanceMethod, method.ReturnType.Name != "String");
                //Set the instruction to be resumed upon not cancelling, if not already
                if (beginIsCancellable && beginResult != null && beginResult.OpCode == OpCodes.Nop)
                {
                    if (method.ReturnType.Name == "Void")
                    {
                        beginResult.OpCode = OpCodes.Brtrue_S;
                        beginResult.Operand = insFirstForMethod;
                    }
                    else if (method.ReturnType.Name == "String")
                    {
                        var il = wrapped.Body.GetILProcessor();

                        //                        il.InsertBefore(beginResult, il.Create(OpCodes.

                        beginResult.OpCode = OpCodes.Brtrue;
                        beginResult.Operand = insFirstForMethod;
                    }
                }

                if (end != null)
                {
                    wrapped.InjectEndCallback(end, instanceMethod);
                }

                wrapped.InjectMethodEnd(noEndHandling);

                if (method.IsVirtual)
                {
                    method.IsVirtual = false;
                }

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

                return wrapped;
            }
            else throw new NotSupportedException("Non Void methods not yet supported");
        }

        public static MethodDefinition ReplaceInstanceMethod(this MethodDefinition method, MethodReference newMethod)
        {
            if (!method.HasBody) throw new InvalidOperationException("Method must have a body.");
            if (method.ReturnType.Name == "Void" || method.ReturnType.Name == "String")
            {
                //Create the new replacement method
                var wrapped = new MethodDefinition(method.Name, method.Attributes, method.ReturnType);
                var instanceMethod = (method.Attributes & MethodAttributes.Static) == 0;

                //Rename the existing method, and replace it
                method.Name = method.Name + WrappedMethodNameSuffix;
                method.ReplaceWith(wrapped);

                //Copy over parameters
                if (method.HasParameters)
                    foreach (var prm in method.Parameters)
                    {
                        wrapped.Parameters.Add(prm);
                    }

                //Place the new method in the declaring type of the method we are cloning
                method.DeclaringType.Methods.Add(wrapped);

                var beginResult = wrapped.InjectBeginCallback(newMethod, instanceMethod, false);
                beginResult.OpCode = OpCodes.Ret;

                if (method.IsVirtual)
                {
                    method.IsVirtual = false;
                }

                return wrapped;
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
                        VariableDefinition vr1;//, vr2;
                        method.Body.Variables.Add(vr1 = new VariableDefinition(method.ReturnType));
                        method.Body.Variables.Add(new VariableDefinition(method.ReturnType));

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

        public static Instruction InsertCancellableMethodBefore(this MethodDefinition method, Instruction ins, MethodReference callback)
        {
            var il = method.Body.GetILProcessor();

            Instruction call;
            il.InsertBefore(ins, call = il.Create(OpCodes.Call, callback));
            //            il.InsertBefore(ins, il.Create(OpCodes.Pop));
            il.InsertBefore(ins, il.Create(OpCodes.Brtrue, ins));
            il.InsertBefore(ins, il.Create(OpCodes.Ret));

            return call;
        }


        /// <summary>
        /// Swaps a Tile array type to an array of the replacement type
        /// </summary>
        /// <returns>The to vanilla reference.</returns>
        /// <param name="input">Input.</param>
        /// <param name="replacement">Replacement.</param>
        private static TypeReference SwapToVanillaReference(TypeReference input, TypeReference replacement, string lookFor)
        {
            if (input.FullName == lookFor)
            {
                return replacement;
            }
            else if (input is ArrayType)
            {
                var at = input as ArrayType;
                if (at.ElementType.FullName == lookFor)
                {
                    var nt = new ArrayType(replacement);
                    nt.Dimensions.Clear();

                    foreach (var dm in at.Dimensions)
                    {
                        nt.Dimensions.Add(dm);
                    }

                    return input.Module.Import(nt);
                }
            }

            return input;
        }

        public static void TryReplaceArrayWithClassInstance(this ModuleDefinition module, TypeDefinition lookFor, TypeDefinition replacementType, TypeDefinition replacementDespatcher, string callbackSuffix)
        {
            foreach (var ty in module.Types)
            {
                ty.TryReplaceArrayWithClassInstance(lookFor, replacementType, replacementDespatcher, callbackSuffix);
            }
        }

        public static void TryReplaceArrayWithClassInstance(this TypeDefinition typeToSearch, TypeDefinition lookFor, TypeDefinition replacementType, TypeDefinition replacementDespatcher, string callbackSuffix)
        {
            var vt = typeToSearch.Module.Import(replacementType);

            var setCall = typeToSearch.Module.Import(replacementDespatcher.Methods.SingleOrDefault(x => x.Name == "Set" + callbackSuffix));
            var getCall = typeToSearch.Module.Import(replacementDespatcher.Methods.SingleOrDefault(x => x.Name == "Get" + callbackSuffix));

            if (typeToSearch != lookFor)
            {
                if (typeToSearch.HasFields)
                    foreach (var fld in typeToSearch.Fields)
                    {
                        fld.FieldType = SwapToVanillaReference(fld.FieldType, vt, lookFor.FullName);
                    }

                if (typeToSearch.HasProperties)
                    foreach (var prop in typeToSearch.Properties)
                    {
                        prop.PropertyType = SwapToVanillaReference(prop.PropertyType, vt, lookFor.FullName);
                    }

                foreach (var mth in typeToSearch.Methods)
                {
                    if (mth.HasParameters)
                    {
                        foreach (var prm in mth.Parameters)
                        {
                            prm.ParameterType = SwapToVanillaReference(prm.ParameterType, vt, lookFor.FullName);
                        }
                    }

                    if (mth.HasBody)
                    {
                        if (mth.Body.HasVariables)
                        {
                            foreach (var vrb in mth.Body.Variables)
                            {
                                vrb.VariableType = SwapToVanillaReference(vrb.VariableType, vt, lookFor.FullName);
                            }
                        }

                        if (mth.Body.Instructions != null)
                        {
                            for (var i = mth.Body.Instructions.Count - 1; i > 0; i--)
                            {
                                var ins = mth.Body.Instructions[i];

                                {
                                    if (ins.OpCode == OpCodes.Call && ins.Operand is MemberReference)
                                    {
                                        var mr = ins.Operand as MemberReference;

                                        if (setCall != null && mr.Name == "Set" && mr.DeclaringType is ArrayType && (mr.DeclaringType as ArrayType).ElementType.FullName == replacementType.FullName)
                                        {
                                            ins.Operand = typeToSearch.Module.Import(setCall);
                                        }

                                        if (getCall != null && mr.Name == "Get" && mr.DeclaringType is ArrayType && (mr.DeclaringType as ArrayType).ElementType.FullName == replacementType.FullName)
                                        {
                                            ins.Operand = typeToSearch.Module.Import(getCall);
                                        }
                                    }
                                }


                                if (ins.Operand is MethodReference)
                                {
                                    var meth = ins.Operand as MethodReference;
                                    if (meth.DeclaringType.FullName == lookFor.FullName)
                                    {
                                        if (meth.Name == ".ctor")
                                        {
                                            ins.Operand = typeToSearch.Module.Import(vt.Resolve().Methods.Single(x => x.Name == meth.Name && x.Parameters.Count == meth.Parameters.Count));
                                            continue;
                                        }

                                        ins.Operand = typeToSearch.Module.Import(vt.Resolve().Methods.Single(x => x.Name == meth.Name && x.Parameters.Count == meth.Parameters.Count));
                                        continue;
                                    }
                                    else if (meth.DeclaringType is ArrayType)
                                    {
                                        var at = meth.DeclaringType as ArrayType;
                                        if (at.ElementType.FullName == lookFor.FullName)
                                        {
                                            meth.DeclaringType = SwapToVanillaReference(meth.DeclaringType, vt, lookFor.FullName);
                                        }
                                    }

                                    if (meth.HasParameters)
                                        foreach (var prm in meth.Parameters)
                                        {
                                            prm.ParameterType = SwapToVanillaReference(prm.ParameterType, vt, lookFor.FullName);
                                        }

                                    meth.ReturnType = SwapToVanillaReference(meth.ReturnType, vt, lookFor.FullName);
                                    meth.MethodReturnType.ReturnType = SwapToVanillaReference(meth.MethodReturnType.ReturnType, vt, lookFor.FullName);
                                }
                                else if (ins.Operand is TypeReference)
                                {
                                    var typ = ins.Operand as TypeReference;
                                    if (typ.FullName == lookFor.FullName)
                                    {
                                        throw new NotImplementedException();
                                    }
                                    else if (typ is ArrayType)
                                    {
                                        var at = typ as ArrayType;
                                        if (at.ElementType.FullName == lookFor.FullName)
                                        {
                                            throw new NotImplementedException();
                                        }
                                    }
                                }
                                else if (ins.Operand is FieldReference)
                                {
                                    var fld = ins.Operand as FieldReference;
                                    if (fld.DeclaringType.FullName == lookFor.FullName)
                                    {
                                        //Now, instead map to our property methods

                                        var il = mth.Body.GetILProcessor();
                                        if (ins.OpCode == OpCodes.Ldfld)
                                        {
#if MEM_TILE_IS_VANILLA
                                            //Get
                                            var prop = _asm.MainModule.Import(vt.Resolve().Fields.Single(x => x.Name == fld.Name));
                                            ins.Operand = prop;
#else
                                            //Get
                                            var prop = typeToSearch.Module.Import(vt.Resolve().Properties.Single(x => x.Name == fld.Name).GetMethod);

                                            il.Replace(ins, il.Create(OpCodes.Callvirt, prop));
#endif
                                        }
                                        else if (ins.OpCode == OpCodes.Stfld)
                                        {
#if MEM_TILE_IS_VANILLA
                                            //Set
                                            var prop = _asm.MainModule.Import(vt.Resolve().Fields.Single(x => x.Name == fld.Name));
                                            ins.Operand = prop;
#else
                                            //Set
                                            var prop = typeToSearch.Module.Import(vt.Resolve().Properties.Single(x => x.Name == fld.Name).SetMethod);

                                            il.Replace(ins, il.Create(OpCodes.Callvirt, prop));
#endif
                                        }
                                        else
                                        {
                                            throw new NotImplementedException();
                                        }
                                    }
                                    else if (fld.DeclaringType is ArrayType)
                                    {
                                        var at = fld.DeclaringType as ArrayType;
                                        if (at.ElementType.FullName == lookFor.FullName)
                                        {
                                            throw new NotImplementedException();
                                        }
                                    }
                                }
                                else if (ins.Operand is PropertyReference)
                                {
                                    throw new NotImplementedException();
                                }
                                else if (ins.Operand is VariableReference)
                                {
                                    var vrb = ins.Operand as VariableReference;
                                    vrb.VariableType = SwapToVanillaReference(vrb.VariableType, vt, lookFor.FullName);
                                }
                                else if (ins.Operand is MemberReference)
                                {
                                    throw new NotImplementedException();
                                }
                            }
                        }
                    }
                }
            }

            if (typeToSearch.HasNestedTypes)
                foreach (var nt in typeToSearch.NestedTypes)
                    nt.TryReplaceArrayWithClassInstance(lookFor, replacementType, replacementDespatcher, callbackSuffix);
        }
    }
}

