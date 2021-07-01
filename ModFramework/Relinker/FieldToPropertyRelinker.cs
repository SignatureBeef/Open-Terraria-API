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
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ModFramework.Relinker
{
    [MonoMod.MonoModIgnore]
    public class FieldToPropertyRelinker : RelinkTask
    {
        FieldDefinition Field { get; }
        PropertyDefinition Property { get; }

        MethodReference getReference;
        MethodReference setReference;
        TypeReference returnTypeReference;

        public FieldToPropertyRelinker(FieldDefinition field, PropertyDefinition property)
        {
            this.Field = field;
            this.Property = property;

            if (this.Property.GetMethod is not null)
            {
                this.getReference = field.Module.ImportReference(this.Property.GetMethod);
                this.returnTypeReference = field.Module.ImportReference(this.Property.GetMethod.ReturnType);
            }

            if (this.Property.SetMethod is not null)
                this.setReference = field.Module.ImportReference(this.Property.SetMethod);

            Console.WriteLine($"[ModFw] Relinking to property {field.FullName}=>{property.FullName}");
        }

        TRef ResolveReference<TRef>(TRef reference)
            where TRef : MemberReference
        {

            return reference;
        }

        public override void Relink(MethodBody body, Instruction instr)
        {
            switch (instr.OpCode.OperandType)
            {
                case OperandType.InlineField:
                    if (instr.Operand is FieldReference field)
                    {
                        if (field.DeclaringType.FullName == this.Field.DeclaringType.FullName
                            || field.DeclaringType.FullName == this.Property.DeclaringType.FullName
                        )
                        {
                            if (field.Name == this.Field.Name || field.Name == this.Property.Name)
                            {
                                if (body.Method == this.Property.GetMethod || body.Method == this.Property.SetMethod)
                                    return;

                                if (instr.OpCode == OpCodes.Ldfld || instr.OpCode == OpCodes.Ldsfld)
                                {
                                    instr.OpCode = OpCodes.Call;
                                    instr.Operand = ResolveReference(this.getReference);
                                }
                                else if (instr.OpCode == OpCodes.Stfld || instr.OpCode == OpCodes.Stsfld)
                                {
                                    instr.OpCode = OpCodes.Call;
                                    instr.Operand = ResolveReference(this.setReference);
                                }
                                else if (instr.OpCode == OpCodes.Ldflda)
                                {
                                    instr.OpCode = OpCodes.Call;
                                    instr.Operand = ResolveReference(this.getReference);

                                    var vrb = new VariableDefinition(ResolveReference(this.returnTypeReference));
                                    body.Variables.Add(vrb);

                                    var ilp = body.GetILProcessor();

                                    ilp.InsertAfter(instr, ilp.Create(OpCodes.Ldloca, vrb));
                                    ilp.InsertAfter(instr, ilp.Create(OpCodes.Stloc, vrb));
                                }
                                else throw new NotImplementedException();
                            }
                        }
                    }
                    break;
            }
        }
    }
}
