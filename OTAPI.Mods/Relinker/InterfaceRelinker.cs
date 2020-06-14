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
using System;
using System.Linq;

namespace OTAPI.Mods.Relinker
{
    public static class InterfaceRelinkerExtensions
    {
        public static TypeReference GetRedirectedReference(this RelinkTask task, TypeReference searchType)
        {
            return (task.RelinkProvider.Tasks
                .FirstOrDefault(t => t is InterfaceRelinker relinker
                    && relinker.SearchType.FullName == searchType.FullName
                ) as InterfaceRelinker
            )?.ReplacementType;
        }
    }

    public class InterfaceRelinker : RelinkTask
    {
        public TypeReference SearchType { get; set; }
        public TypeReference ReplacementType { get; set; }
        public override int Order => 150;

        public InterfaceRelinker(TypeReference searchType, TypeReference replacementType)
        {
            SearchType = searchType;
            ReplacementType = replacementType;
            Console.WriteLine($"[OTAPI] Relinking interface {searchType.FullName}=>{replacementType.FullName}");
        }

        public override void Relink(MethodDefinition method)
        {
            if (method.ReturnType.FullName == SearchType.FullName)
                method.ReturnType = (ReplacementType);

            if (method.ReturnType is ArrayType arrayType)
                if (arrayType.ElementType.FullName == SearchType.FullName)
                    method.ReturnType = new ArrayType((ReplacementType), arrayType.Rank);
        }

        public override void Relink(MethodDefinition method, VariableDefinition variable)
        {
            if (variable.VariableType.FullName == SearchType.FullName)
                variable.VariableType = (ReplacementType);

            if (variable.VariableType is ArrayType arrayType)
                if (arrayType.ElementType.FullName == SearchType.FullName)
                    variable.VariableType = new ArrayType((ReplacementType), arrayType.Rank);
        }

        public override void Relink(MethodDefinition method, ParameterDefinition parameter)
        {
            if (parameter.ParameterType.FullName == SearchType.FullName)
                parameter.ParameterType = (ReplacementType);

            if (parameter.ParameterType is ArrayType arrayType)
                if (arrayType.ElementType.FullName == SearchType.FullName)
                    parameter.ParameterType = new ArrayType((ReplacementType), arrayType.Rank);
        }
        public override void Relink(PropertyDefinition property)
        {
            if (property.PropertyType.FullName == SearchType.FullName)
                property.PropertyType = (ReplacementType);

            if (property.PropertyType is ArrayType arrayType)
                if (arrayType.ElementType.FullName == SearchType.FullName)
                    property.PropertyType = new ArrayType((ReplacementType), arrayType.Rank);
        }

        public override void Relink(FieldDefinition field)
        {
            if (field.FieldType.FullName == SearchType.FullName)
                field.FieldType = (ReplacementType);

            if (field.FieldType is GenericInstanceType genericInstanceType)
                if (genericInstanceType.HasGenericArguments)
                    for (var i = 0; i < genericInstanceType.GenericArguments.Count; i++)
                        if (genericInstanceType.GenericArguments[i].FullName == SearchType.FullName)
                            genericInstanceType.GenericArguments[i] = (ReplacementType);

            if (field.FieldType is ArrayType arrayType)
                if (arrayType.ElementType.FullName == SearchType.FullName)
                    field.FieldType = new ArrayType((ReplacementType), arrayType.Rank);
        }

        public override void Relink(MethodBody body, Instruction instr)
        {
            var methodIsDeclaredByType = body.Method.DeclaringType.FullName == SearchType.FullName;

            if (body.Method.Name == "FixHeart")
            {

            }

            if (instr.Operand is MethodReference methodRef)
            {
                // interreference is where the same class references itself, but since we are switching types
                // we must only do so when its not 'this'.
                var isInterreference = RelinkProvider.AllowInterreferenceReplacements && methodIsDeclaredByType
                        && (instr.OpCode == OpCodes.Call || instr.OpCode == OpCodes.Callvirt)
                        && instr.Previous?.OpCode == OpCodes.Ldarg_1
                        && methodRef.HasThis;

                if (instr.OpCode == OpCodes.Newobj || (!body.Method.IsStatic && methodIsDeclaredByType && !isInterreference))
                    return;

                if (methodRef.DeclaringType.FullName == SearchType.FullName && methodRef.HasThis)
                {
                    var mr = new MethodReference(methodRef.Name, methodRef.ReturnType, ReplacementType);
                    mr.HasThis = methodRef.HasThis;

                    foreach (var prm in methodRef.Parameters)
                    {
                        mr.Parameters.Add(
                            new ParameterDefinition(prm.Name, prm.Attributes, prm.ParameterType)
                        );
                    }

                    mr.ReturnType = methodRef.ReturnType;

                    instr.Operand = body.Method.Module.ImportReference(mr);
                    // methodRef.DeclaringType = (ReplacementType);
                    // instr.Operand = body.Method.Module.ImportReference((IMetadataTokenProvider)methodRef);

                    methodRef = mr;
                }

                if (methodRef.ReturnType.FullName == SearchType.FullName)
                    methodRef.ReturnType = (ReplacementType);

                if (methodRef.HasParameters)
                    for (var i = 0; i < methodRef.Parameters.Count; i++)
                        if (methodRef.Parameters[i].ParameterType.FullName == SearchType.FullName)
                            methodRef.Parameters[i].ParameterType = (ReplacementType);

                if (methodRef.DeclaringType is GenericInstanceType genericInstanceType)
                    if (genericInstanceType.HasGenericArguments)
                        for (var i = 0; i < genericInstanceType.GenericArguments.Count; i++)
                            if (genericInstanceType.GenericArguments[i].FullName == SearchType.FullName)
                                genericInstanceType.GenericArguments[i] = (ReplacementType);

                if (methodRef.DeclaringType is ArrayType arrayType)
                    if (arrayType.ElementType.FullName == SearchType.FullName)
                        methodRef.DeclaringType = new ArrayType((ReplacementType), arrayType.Rank);

                // upgrade call to callvirt
                if (instr.OpCode == OpCodes.Call && methodRef.DeclaringType.FullName == ReplacementType.FullName)
                    instr.OpCode = OpCodes.Callvirt;
            }

            if (instr.Operand is FieldReference fieldReference)
            {
                if (methodIsDeclaredByType) return;

                if (fieldReference.DeclaringType.FullName == SearchType.FullName)
                    fieldReference.DeclaringType = (ReplacementType);

                // relink calls to the field
                if (fieldReference.FieldType is GenericInstanceType genericInstanceType1)
                    if (genericInstanceType1.HasGenericArguments)
                        for (var i = 0; i < genericInstanceType1.GenericArguments.Count; i++)
                            if (genericInstanceType1.GenericArguments[i].FullName == SearchType.FullName)
                                genericInstanceType1.GenericArguments[i] = (ReplacementType);

                if (fieldReference.FieldType is ArrayType arrayType)
                    if (arrayType.ElementType.FullName == SearchType.FullName)
                        fieldReference.FieldType = new ArrayType((ReplacementType), arrayType.Rank);
            }
        }
    }
}
