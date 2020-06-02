using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace OTAPI
{
    public class FieldToPropertyRemapper
    {
        FieldDefinition Field { get; set; }
        PropertyDefinition Property { get; set; }

        public FieldToPropertyRemapper(FieldDefinition field, PropertyDefinition property)
        {
            this.Field = field;
            this.Property = property;
        }

        [RemapHook]
        public void Remap(Instruction ins, MethodDefinition method)
        {
            switch (ins.OpCode.OperandType)
            {
                case OperandType.InlineField:
                    if (ins.Operand is FieldReference field)
                    {
                        if (field.DeclaringType.FullName == this.Field.DeclaringType.FullName)
                        {
                            if (field.Name == this.Field.Name || field.Name == this.Property.Name)
                            {
                                if (method == this.Property.GetMethod || method == this.Property.SetMethod)
                                    return;

                                if (ins.OpCode == OpCodes.Ldfld)
                                {
                                    ins.OpCode = OpCodes.Call;
                                    ins.Operand = this.Property.GetMethod;
                                }
                                else if (ins.OpCode == OpCodes.Stfld)
                                {
                                    ins.OpCode = OpCodes.Call;
                                    ins.Operand = this.Property.SetMethod;
                                }
                                else
                                {

                                }
                            }
                        }
                    }
                    break;
            }
        }
    }

    public static class PropertyEmitter
    {
        const MethodAttributes DefaultMethodAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

        public static PropertyDefinition RemapAsProperty(this FieldDefinition field, Remapper remapper)
        {
            var property = new PropertyDefinition(field.Name, PropertyAttributes.None, field.FieldType);

            property.HasThis = true;
            property.GetMethod = field.GenerateGetter();
            property.SetMethod = field.GenerateSetter();

            field.Name = $"<{field.Name}>k__BackingField";
            field.Attributes = FieldAttributes.Private;

            //Add the CompilerGeneratedAttribute or if you decompile the getter body will be shown
            field.CustomAttributes.Add(new CustomAttribute(
                field.DeclaringType.Module
                    .GetCoreLibMethod("System.Runtime.CompilerServices", "CompilerGeneratedAttribute", ".ctor")
            ));

            field.DeclaringType.Properties.Add(property);

            // add a task to rewrite the field accessors to properties
            remapper.Tasks.Add(new FieldToPropertyRemapper(field, property));

            return property;
        }

        public static void RemapFieldsToProperties(this TypeDefinition type, Remapper remapper)
        {
            foreach (var field in type.Fields.Where(x => !x.HasConstant))
            {
                // rename to the backing field
                field.RemapAsProperty(remapper);
            }
        }

        public static MethodDefinition GenerateGetter(this FieldDefinition field)
        {
            //Create the method definition
            var method = new MethodDefinition("get_" + field.Name, DefaultMethodAttributes, field.FieldType);

            //Create the il processor so we can alter il
            var il = method.Body.GetILProcessor();

            //Load the current type instance if required
            if (!field.IsStatic)
                il.Append(il.Create(OpCodes.Ldarg_0));

            //Load the backing field
            il.Append(il.Create(OpCodes.Ldfld, field));
            //Return the backing fields value
            il.Append(il.Create(OpCodes.Ret));

            //Set basic getter method details 
            method.Body.InitLocals = true;
            method.SemanticsAttributes = MethodSemanticsAttributes.Getter;
            method.IsGetter = true;

            //Add the CompilerGeneratedAttribute or if you decompile the getter body will be shown
            method.CustomAttributes.Add(new CustomAttribute(
                field.DeclaringType.Module
                    .GetCoreLibMethod("System.Runtime.CompilerServices", "CompilerGeneratedAttribute", ".ctor")
            ));

            field.DeclaringType.Methods.Add(method);

            return method;
        }

        public static MethodDefinition GenerateSetter(this FieldDefinition field)
        {
            //Create the method definition
            var method = new MethodDefinition("set_" + field.Name, DefaultMethodAttributes, field.DeclaringType.Module.TypeSystem.Void);

            //Setters always have a 'value' variable, but it's really just a parameter. We need to add this.
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, field.FieldType));

            //Create the il processor so we can alter il
            var il = method.Body.GetILProcessor();

            //Load the current type instance if required
            if (!field.IsStatic)
                il.Append(il.Create(OpCodes.Ldarg_0));
            //Load the 'value' parameter we added (alternatively, we could do il.Create(OpCodes.Ldarg, <field definition>)
            il.Append(il.Create(OpCodes.Ldarg_1));
            //Store the parameters value into the backing field
            il.Append(il.Create(OpCodes.Stfld, field));
            //Return from the method as we are done.
            il.Append(il.Create(OpCodes.Ret));

            //Set basic setter method details 
            method.Body.InitLocals = true;
            method.SemanticsAttributes = MethodSemanticsAttributes.Setter;
            method.IsSetter = true;

            //Add the CompilerGeneratedAttribute or if you decompile the getter body will be shown
            method.CustomAttributes.Add(new CustomAttribute(
                field.DeclaringType.Module
                    .GetCoreLibMethod("System.Runtime.CompilerServices", "CompilerGeneratedAttribute", ".ctor")
            ));

            field.DeclaringType.Methods.Add(method);

            return method;
        }
    }
}