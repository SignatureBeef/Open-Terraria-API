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

        public FieldToPropertyRelinker(FieldDefinition field, PropertyDefinition property)
        {
            this.Field = field;
            this.Property = property;

            Console.WriteLine($"[ModFw] Relinking to property {field.FullName}=>{property.FullName}");
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

                                if (instr.OpCode == OpCodes.Ldfld)
                                {
                                    instr.OpCode = OpCodes.Call;
                                    instr.Operand = this.Property.GetMethod;
                                }
                                else if (instr.OpCode == OpCodes.Stfld)
                                {
                                    instr.OpCode = OpCodes.Call;
                                    instr.Operand = this.Property.SetMethod;
                                }
                                else if (instr.OpCode == OpCodes.Ldflda)
                                {
                                    instr.OpCode = OpCodes.Call;
                                    instr.Operand = this.Property.GetMethod;

                                    var vrb = new VariableDefinition(this.Property.GetMethod.ReturnType);
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
