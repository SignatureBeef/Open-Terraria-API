using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTAPI.Patcher.Extensions
{
    public static partial class WrappingExtensions
    {
        const String WrappedMethodNameSuffix = "Direct";

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

        public static Instruction CreateParameterInstruction(this MethodDefinition method, int index)
        {
            return Instruction.Create(OpCodes.Ldarg, method.Parameters[index]);
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
                {
                    var offset = instanceMethod ? 1 : 0;
                    if (begin.Parameters[i + offset].ParameterType.IsByReference)
                    {
                        //new ParameterDefinition(method.Parameters[i].Name, method.Parameters[i].Attributes, new ByReferenceType(method.Parameters[i].ParameterType))
                        il.Emit(OpCodes.Ldarga, method.Parameters[i]);
                    }
                    else il.Emit(OpCodes.Ldarg, method.Parameters[i]);
                }
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

        public static Instruction InjectCallback(this MethodDefinition method, MethodReference callback, bool instanceMethod, bool isCancellable = false, Instruction target = null)
        {
            Instruction firstInstruction = null;
            Instruction ins = null;

            //Import the callbacks to the calling methods assembly
            var impBegin = method.Module.Import(callback);

            //If we don't have a target instruction already then we will default to the start of the method
            if (target == null)
                target = method.Body.Instructions.First();

            //            var instanceMethod = (method.Attributes & MethodAttributes.Static) == 0;

            //Now, create the IL body
            var il = method.Body.GetILProcessor();

            //Execute the begin hook
            if (instanceMethod)
            {
                ins = il.Create(OpCodes.Ldarg_0);
                if (firstInstruction == null)
                    firstInstruction = ins;
                il.InsertBefore(target, ins);
            }
            if (method.HasParameters)
                for (var i = 0; i < method.Parameters.Count; i++)
                {
                    ins = null;
                    var offset = instanceMethod ? 1 : 0;
                    if (callback.Parameters[i + offset].ParameterType.IsByReference)
                    {
                        il.InsertBefore(target, ins = il.Create(OpCodes.Ldarga, method.Parameters[i]));
                    }
                    else il.InsertBefore(target, ins = il.Create(OpCodes.Ldarg, method.Parameters[i]));

                    if (firstInstruction == null)
                        firstInstruction = ins;
                }


            il.InsertBefore(target, ins = il.Create(OpCodes.Call, impBegin));
            if (firstInstruction == null)
                firstInstruction = ins;

            //Create the cancel return if required.

            if (isCancellable)
            {
                firstInstruction = il.Create(OpCodes.Nop);
                il.InsertBefore(target, ins = firstInstruction);
                if (firstInstruction == null)
                    firstInstruction = ins;

                //Emit the cancel handling
                if (method.ReturnType.Name == "Void")
                {
                    il.InsertBefore(target, ins = il.Create(OpCodes.Ret));
                    if (firstInstruction == null)
                        firstInstruction = ins;
                }
                else
                {
                    //Return a default value
                    VariableDefinition vr1;
                    method.Body.Variables.Add(vr1 = new VariableDefinition(method.ReturnType));

                    //Initialise the variable
                    il.InsertBefore(target, ins = il.Create(OpCodes.Ldloca_S, vr1));
                    if (firstInstruction == null)
                        firstInstruction = ins;
                    il.InsertBefore(target, il.Create(OpCodes.Initobj, method.ReturnType));
                    il.InsertBefore(target, il.Create(OpCodes.Ldloc, vr1));

                    il.InsertBefore(target, il.Create(OpCodes.Ret));
                }
            }
            else if (impBegin.ReturnType.Name != "Void")
            {
                firstInstruction = il.Create(OpCodes.Pop);
                il.InsertBefore(target, ins = firstInstruction);
                if (firstInstruction == null)
                    firstInstruction = ins;
            }

            target.ReplaceTransfer(firstInstruction, method);

            return firstInstruction;
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

                return wrapped;
            }
            else throw new NotSupportedException("Non Void methods not yet supported");
        }
    }
}
