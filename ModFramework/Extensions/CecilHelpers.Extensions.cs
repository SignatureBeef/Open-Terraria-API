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
using System.Linq;
using System.Linq.Expressions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace ModFramework
{
    [MonoMod.MonoModIgnore]
    public static partial class CecilHelpersExtensions
    {
        public static ILCursor GetILCursor(this MonoMod.MonoModder modder, Expression<Action> reference)
            => new ILCursor(new ILContext(modder.Module.GetDefinition<MethodDefinition>(reference)) { ReferenceBag = RuntimeILReferenceBag.Instance });

        public static MethodDefinition GetMethodDefinition(this MonoMod.MonoModder modder, Expression<Action> reference)
            => modder.Module.GetDefinition<MethodDefinition>(reference);
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

        public static TypeDefinition GetDefinition<TType>(this ModuleDefinition module)
        {
            var target = typeof(TType).FullName;
            return module.Types.Single(t => t.FullName == target);
        }

        public static TReturn GetDefinition<TReturn>(this IMetadataTokenProvider token, LambdaExpression reference)
        {
            //=> (TReturn)token.GetModule().MetadataResolver.Resolve(token.GetMemberReference(reference));
            var memberReference = token.GetMemberReference(reference);

            // try and resolve back to the token module rather than the reference module.
            if(memberReference is MethodReference methodReference)
            {
                var module = token.GetModule();
                methodReference.DeclaringType = module.GetType(methodReference.DeclaringType.FullName);
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
                // find the expression method in the meta, matching on the parameter count/types
                var mce = reference.Body as MethodCallExpression;
                if (mce != null)
                    return module.ImportReference(mce.Method);
            }

            var me = reference.Body as MemberExpression;
            if (me != null)
            {
                if (me.Member is System.Reflection.FieldInfo field)
                    return module.ImportReference(field);
            }

            throw new System.Exception($"Unable to find expression in assembly");
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
    }
}