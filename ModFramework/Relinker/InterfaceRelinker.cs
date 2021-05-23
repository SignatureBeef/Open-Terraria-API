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
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModFramework.Relinker
{
    [MonoMod.MonoModIgnore]
    public class InterfaceRelinker : RelinkTask
    {
        public TypeReference SearchType { get; set; }
        public TypeReference ReplacementType { get; set; }
        public override int Order => 150;

        public InterfaceRelinker(TypeReference searchType, TypeReference replacementType)
        {
            SearchType = searchType;
            ReplacementType = replacementType;
            Console.WriteLine($"[ModFw] Relinking interface {searchType.FullName}=>{replacementType.FullName}");
        }

        public override void Relink(TypeDefinition type)
        {
            base.Relink(type);

            if (type.BaseType != null)
                ResolveType(type.BaseType, nr => type.BaseType = nr);

            if (type.HasNestedTypes)
                foreach (var nt in type.NestedTypes)
                {
                    Relink(nt);
                }
        }

        public override void Relink(MethodDefinition method)
        {
            ResolveType(method.ReturnType, nr => method.ReturnType = nr);

            foreach (var prm in method.Parameters)
                Relink(method, prm);

            if (method.HasBody)
                foreach (var vrb in method.Body.Variables)
                    Relink(method, vrb);
        }

        public override void Relink(MethodDefinition method, VariableDefinition variable)
        {
            ResolveType(variable.VariableType, nr => variable.VariableType = nr);
        }

        public override void Relink(MethodDefinition method, ParameterDefinition parameter)
        {
            ResolveType(parameter.ParameterType, nr => parameter.ParameterType = nr);
        }

        public override void Relink(PropertyDefinition property)
        {
            ResolveType(property.PropertyType, nr => property.PropertyType = nr);
        }

        public override void Relink(FieldDefinition field)
        {
            ResolveType(field.FieldType, nr => field.FieldType = nr);
        }

        void ResolveType<TRef>(TRef typeRef, Action<TRef> update)
             where TRef : TypeReference
        {
            var original = typeRef;
            if (typeRef.FullName == SearchType.FullName)
            {
                typeRef = (TRef)ReplacementType;
            }
            else if (typeRef is GenericInstanceType genericInstanceType)
            {
                if (genericInstanceType.HasGenericArguments)
                    for (var i = 0; i < genericInstanceType.GenericArguments.Count; i++)
                        ResolveType(
                            genericInstanceType.GenericArguments[i],
                            nr => genericInstanceType.GenericArguments[i] = nr
                        );
            }
            else if (typeRef is ArrayType arrayType)
            {
                if (arrayType.ElementType.FullName == SearchType.FullName)
                    typeRef = (TRef)(object)new ArrayType(ReplacementType, arrayType.Rank);
            }
            else if (typeRef is ByReferenceType byRefType)
            {
                if (byRefType.ElementType.FullName == SearchType.FullName)
                    typeRef = (TRef)(object)new ByReferenceType(ReplacementType);
            }

            if (typeRef.HasGenericParameters)
                for (int i = 0; i < typeRef.GenericParameters.Count; i++)
                {
                    ResolveType(
                        typeRef.GenericParameters[i],
                        nr => typeRef.GenericParameters[i] = nr
                    );
                }

            if (typeRef != original)
            {
                update(typeRef);
            }
        }

        public override void Relink(MethodBody body, Instruction instr)
        {
            var methodIsDeclaredByType = body.Method.DeclaringType.FullName == SearchType.FullName;

            if (instr.Operand is MethodReference methodRef)
            {
                var methodRefIsDeclaredByType = methodRef.DeclaringType.FullName == SearchType.FullName;
                var isSelfDefined = SearchType.Resolve().Methods.Any(x => x.FullName == methodRef.FullName);

                // if the method is in the class we are replacing, and the method is self defined we dont need to replace it
                if (methodIsDeclaredByType && isSelfDefined)
                {
                    var cnt = body.Method.GetStack();
                    var offset = cnt.Single(x => x.Ins == instr);
                    var start = offset.FindCallStart();

                    bool isExternal = false;
                    if (start.Ins?.Operand is ParameterReference par)
                    {
                        isExternal = par.ParameterType.FullName != SearchType.FullName;
                    }
                    else if (start.Ins?.OpCode == OpCodes.Ldarg_0)
                    {
                        if (body.Method.IsStatic)
                        {
                            isExternal = body.Method.Parameters[0].ParameterType.FullName != SearchType.FullName;
                        }
                        else
                        {
                            isExternal = body.Method.DeclaringType.FullName != SearchType.FullName;
                        }
                    }
                    else if (start.Ins?.OpCode == OpCodes.Ldarg_1)
                    {
                        if (body.Method.IsStatic)
                        {
                            isExternal = body.Method.Parameters[1].ParameterType.FullName != SearchType.FullName;
                        }
                        else
                        {
                            isExternal = body.Method.Parameters[0].ParameterType.FullName != SearchType.FullName;
                        }
                    }
                    else if (start.Ins?.OpCode == OpCodes.Ldarg_2)
                    {
                        if (body.Method.IsStatic)
                        {
                            isExternal = body.Method.Parameters[2].ParameterType.FullName != SearchType.FullName;
                        }
                        else
                        {
                            isExternal = body.Method.Parameters[1].ParameterType.FullName != SearchType.FullName;
                        }
                    }
                    else if (start.Ins?.OpCode == OpCodes.Ldarg_3)
                    {
                        if (body.Method.IsStatic)
                        {
                            isExternal = body.Method.Parameters[3].ParameterType.FullName != SearchType.FullName;
                        }
                        else
                        {
                            isExternal = body.Method.Parameters[2].ParameterType.FullName != SearchType.FullName;
                        }
                    }
                    else if (start.Ins?.OpCode == OpCodes.Ldloc_0)
                    {
                        isExternal = body.Variables[0].VariableType.FullName != SearchType.FullName;
                    }
                    else
                    {

                    }

                    if (!isExternal)
                        return;
                }

                // cannot new up interfaces.
                if (instr.OpCode == OpCodes.Newobj)
                {
                    if (instr.Operand is MethodReference mref)
                    {
                        foreach (var prm in mref.Parameters)
                        {
                            ResolveType(prm.ParameterType, nr => prm.ParameterType = nr);
                        }
                    }
                    return;
                }

                ResolveType(methodRef.ReturnType, nr => methodRef.ReturnType = nr);

                if (methodRef.HasParameters)
                    for (var i = 0; i < methodRef.Parameters.Count; i++)
                    {
                        var prm = methodRef.Parameters[i];

                        ResolveType(prm.ParameterType, nr => prm.ParameterType = nr);
                    }

                ResolveType(methodRef.DeclaringType, nr =>
                {
                    if (methodRef.HasThis)
                    {
                        // this is important not to do.
                        // it appears as if they are cached in memory somewhere, and replacing this will
                        // cause problems for other Operands - so the method ref changes but not in the saved IL
                        //methodRef.DeclaringType = nr;

                        var mr = new MethodReference(methodRef.Name, methodRef.ReturnType, ReplacementType)
                        {
                            HasThis = methodRef.HasThis
                        };
                        foreach (var prm in methodRef.Parameters)
                        {
                            mr.Parameters.Add(prm.Clone());
                        }

                        instr.Operand = body.Method.Module.ImportReference(mr);
                    }
                });

                // upgrade call to callvirt
                if (instr.OpCode == OpCodes.Call && methodRef.DeclaringType.FullName == ReplacementType.FullName)
                    instr.OpCode = OpCodes.Callvirt;

            }

            if (instr.Operand is FieldReference fieldReference)
            {
                var fieldIsDeclaredByType = fieldReference.DeclaringType.FullName == SearchType.FullName;
                var isBackingField = fieldIsDeclaredByType && fieldReference.FullName.Contains("k__BackingField");
                if (isBackingField)
                    return;

                ResolveType(fieldReference.FieldType, nr => fieldReference.FieldType = nr);
                ResolveType(fieldReference.DeclaringType, nr => fieldReference.DeclaringType = nr);
            }

            if (instr.Operand is VariableReference varRef)
            {
                ResolveType(varRef.VariableType, nr => varRef.VariableType = nr);
            }
        }
    }
}
