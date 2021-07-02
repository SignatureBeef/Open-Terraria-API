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
using System.Linq;
using System.Linq.Expressions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace ModFramework
{
    [MonoMod.MonoModIgnore]
    public static class CecilHelpersExtensions
    {
        public static ILCursor GetILCursor(this MonoMod.MonoModder modder, Expression<Action> reference, bool followRedirect = true)
            => new ILCursor(new ILContext(modder.Module.GetDefinition<MethodDefinition>(reference, followRedirect)) { ReferenceBag = RuntimeILReferenceBag.Instance });

        public static ILCursor GetILCursor(this MonoMod.MonoModder modder, MethodDefinition method, bool followRedirect = true)
            => new ILCursor(new ILContext(method) { ReferenceBag = RuntimeILReferenceBag.Instance });

        public static MethodDefinition GetMethodDefinition(this MonoMod.MonoModder modder, Expression<Action> reference, bool followRedirect = true)
            => modder.Module.GetDefinition<MethodDefinition>(reference, followRedirect: followRedirect);
        public static FieldDefinition GetFieldDefinition<TResult>(this MonoMod.MonoModder modder, Expression<Func<TResult>> reference)
            => modder.Module.GetDefinition<FieldDefinition>(reference);

        public static TypeDefinition GetDefinition<TType>(this MonoMod.MonoModder modder)
            => modder.Module.GetDefinition<TType>();

        public static ILCursor GetILCursor(this ModuleDefinition module, Expression<Action> reference)
            => new ILCursor(new ILContext(module.GetDefinition<MethodDefinition>(reference)) { ReferenceBag = RuntimeILReferenceBag.Instance });

        public static ILCursor GetILCursor(this MethodDefinition method)
            => new ILCursor(new ILContext(method) { ReferenceBag = RuntimeILReferenceBag.Instance });

        public static MethodReference GetReference<TReturn>(this ModuleDefinition module, Expression<Func<TReturn>> reference)
            => (MethodReference)module.GetMemberReference(reference);
        public static MemberReference GetReference(this ModuleDefinition module, Expression<Action> reference)
            => module.GetMemberReference(reference);

        public static MethodReference GetReference<TReturn>(this MonoMod.MonoModder modder, Expression<Func<TReturn>> reference)
            => modder.Module.GetReference<TReturn>(reference);
        public static MemberReference GetReference(this MonoMod.MonoModder modder, Expression<Action> reference)
            => modder.Module.GetReference(reference);

        public static TypeDefinition GetDefinition<TType>(this ModuleDefinition module)
        {
            // resolve via the module meta data, otherwise try external refs (ie System.*)
            var target = typeof(TType).FullName;
            var def = module.Types.SingleOrDefault(t => t.FullName == target);
            if (def == null)
            {
                var reference = module.ImportReference(typeof(TType));
                def = reference.Resolve();
            }

            if (def == null)
                throw new Exception($"Failed to resolve type: {typeof(TType).FullName}");

            return def;
        }

        public static TReturn GetDefinition<TReturn>(this IMetadataTokenProvider token, LambdaExpression reference, bool followRedirect = false)
        {
            //=> (TReturn)token.GetModule().MetadataResolver.Resolve(token.GetMemberReference(reference));
            var memberReference = token.GetMemberReference(reference);

            // try and resolve back to the token module rather than the reference module.
            if (memberReference is MethodReference methodReference)
            {
                var module = token.GetModule();
                methodReference.DeclaringType = module.GetType(methodReference.DeclaringType.FullName);

                if (followRedirect)
                {
                    var redirected = methodReference.DeclaringType.Resolve().Methods.SingleOrDefault(m => m.Name == "orig_" + methodReference.Name);
                    if (redirected != null)
                    {
                        return (TReturn)(object)redirected;
                    }
                }

                return (TReturn)(object)module.MetadataResolver.Resolve(methodReference);
            }

            return (TReturn)memberReference.Resolve();
        }

        public static ModuleDefinition GetModule(this IMetadataTokenProvider token) => (token as ModuleDefinition) ?? (token as AssemblyDefinition)?.MainModule;

        public static MemberReference GetMemberReference(this IMetadataTokenProvider token, LambdaExpression reference)
        {
            var module = token.GetModule();
            if (module != null)
            {
                if (reference.Body is MethodCallExpression mce)
                    return module.ImportReference(mce.Method);

                if (reference.Body is MemberExpression me && me.Member is System.Reflection.FieldInfo field)
                    return module.ImportReference(field);

                if (reference.Body is NewExpression ne)
                    return module.ImportReference(ne.Constructor);
            }

            throw new System.Exception("Unable to find expression in assembly");
        }

        public static MethodReference GetCoreLibMethod(this ModuleDefinition module, string @namespace, string type, string method)
        {
            var type_ref = new TypeReference(@namespace, type,
                module.TypeSystem.String.Module,
                module.TypeSystem.CoreLibrary
            );
            return new MethodReference(method, module.TypeSystem.Void, type_ref);
        }

        public static void ReplaceTransfer(this Instruction current, Instruction newTarget, MethodDefinition originalMethod)
        {
            //If a method has a body then check the instruction targets & exceptions
            if (originalMethod.HasBody)
            {
                //Replaces instruction references from the old instruction to the new instruction
                foreach (var ins in originalMethod.Body.Instructions.Where(x => x.Operand == current))
                    ins.Operand = newTarget;

                //If there are exception handlers, it's possible that they will also need to be switched over
                if (originalMethod.Body.HasExceptionHandlers)
                {
                    foreach (var handler in originalMethod.Body.ExceptionHandlers)
                    {
                        if (handler.FilterStart == current) handler.FilterStart = newTarget;
                        if (handler.HandlerEnd == current) handler.HandlerEnd = newTarget;
                        if (handler.HandlerStart == current) handler.HandlerStart = newTarget;
                        if (handler.TryEnd == current) handler.TryEnd = newTarget;
                        if (handler.TryStart == current) handler.TryStart = newTarget;
                    }
                }

                //Update the new target to take the old targets place
                newTarget.Offset = current.Offset;
                newTarget.Offset++;
            }
        }

        /// <summary>
        /// Converts a anonymous type into an Instruction
        /// </summary>
        /// <param name="anon"></param>
        /// <returns></returns>
        public static Instruction AnonymousToInstruction(object anon)
        {
            var annonType = anon.GetType();
            var properties = annonType.GetProperties();

            //An instruction consists of only 1 opcode, or 1 opcode and 1 operation
            if (properties.Length == 0 || properties.Length > 2)
                throw new NotSupportedException("Anonymous instruction expected 1 or 2 properties");

            //Determine the property that contains the OpCode property
            var propOpcode = properties.SingleOrDefault(x => x.PropertyType == typeof(OpCode));
            if (propOpcode == null)
                throw new NotSupportedException("Anonymous instruction expected 1 opcode property");

            //Get the opcode value
            var opcode = (OpCode)propOpcode.GetMethod.Invoke(anon, null);

            //Now determine if we need an operand or not
            Instruction ins = null;
            if (properties.Length == 2)
            {
                //We know we already have the opcode determined, so the second property
                //must be the operand.
                var propOperand = properties.Where(x => x != propOpcode).Single();

                var operand = propOperand.GetMethod.Invoke(anon, null);

                //Now find the Instruction.Create method that takes the same type that is 
                //specified by the operands type.
                //E.g. Instruction.Create(OpCode, FieldReference)
                var instructionMethod = typeof(Instruction).GetMethods()
                    .Where(x => x.Name == "Create")
                    .Select(x => new { Method = x, Parameters = x.GetParameters() })
                    //.Where(x => x.Parameters.Length == 2 && x.Parameters[1].ParameterType == propOperand.PropertyType)
                    .Where(x => x.Parameters.Length == 2 &&
                        (
                            x.Parameters[1].ParameterType.IsAssignableFrom(propOperand.PropertyType)
                            ||
                            x.Parameters[1].ParameterType.IsAssignableFrom(operand.GetType())
                        )
                    )
                    .SingleOrDefault();

                if (instructionMethod == null)
                    throw new NotSupportedException($"Instruction.Create does not support type {propOperand.PropertyType.FullName}");

                //Get the operand value and pass it to the Instruction.Create method to create
                //the instruction.
                //var operand = propOperand.GetMethod.Invoke(anon, null);
                ins = (Instruction)instructionMethod.Method.Invoke(anon, new[] { opcode, operand });
            }
            else
            {
                //No operand required
                ins = Instruction.Create(opcode);
            }

            return ins;
        }

        /// <summary>
        /// Inserts a group of instructions after the target instruction
        /// </summary>
        public static void InsertAfter(this Mono.Cecil.Cil.ILProcessor processor, Instruction target, IEnumerable<Instruction> instructions)
        {
            foreach (var instruction in instructions.Reverse())
            {
                processor.InsertAfter(target, instruction);
            }
        }

        /// <summary>
        /// Inserts a list of anonymous instructions after the target instruction
        /// </summary>
        public static List<Instruction> InsertAfter(this Mono.Cecil.Cil.ILProcessor processor, Instruction target, params object[] instructions)
        {
            var created = new List<Instruction>();
            foreach (var anon in instructions.Reverse())
            {
                var ins = AnonymousToInstruction(anon);
                processor.InsertAfter(target, ins);

                created.Add(ins);
            }

            return created;
        }

        /// <summary>
        /// Inserts a list of anonymous instructions before the target instruction
        /// </summary>
        public static List<Instruction> InsertBefore(this Mono.Cecil.Cil.ILProcessor processor, Instruction target, params object[] instructions)
        {
            var created = new List<Instruction>();
            foreach (var anon in instructions)
            {
                var ins = AnonymousToInstruction(anon);
                processor.InsertBefore(target, ins);

                created.Add(ins);
            }

            return created;
        }


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
                if (predicate(initial.Next)) return initial.Next;
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

        public static bool In(this OpCode opCode, params OpCode[] opCodes)
        {
            return opCodes.Any(oc => oc == opCode);
        }

        public static bool In(this StackBehaviour stackBehaviour, params StackBehaviour[] stackBehaviours)
        {
            return stackBehaviours.Any(sb => sb == stackBehaviour);
        }

        /// <summary>
        /// Empties the method
        /// </summary>
        /// <param name="method"></param>
        public static void ClearBody(this MethodDefinition method)
        {
            method.Body.Instructions.Clear();
            method.Body.ExceptionHandlers.Clear();
            method.Body.Variables.Clear();
            method.EmitDefault();
        }

        /// <summary>
        /// Emits a default return value
        /// </summary>
        public static Instruction EmitDefault(this MethodDefinition method)
        {
            Instruction firstInstruction = null;

            var il = method.Body.GetILProcessor();

            if (method.ReturnType.Name != method.Module.TypeSystem.Void.Name)
            {
                VariableDefinition vr1;
                method.Body.Variables.Add(vr1 = new VariableDefinition(method.ReturnType));

                //Initialise the variable
                il.Append(firstInstruction = il.Create(OpCodes.Ldloca_S, vr1));
                il.Emit(OpCodes.Initobj, method.ReturnType);
                il.Emit(OpCodes.Ldloc, vr1);
            }

            //The method is now complete.
            if (firstInstruction == null)
                il.Append(firstInstruction = il.Create(OpCodes.Ret));
            else il.Emit(OpCodes.Ret);

            return firstInstruction;
        }
    }
}