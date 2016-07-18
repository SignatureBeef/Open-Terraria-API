using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace OTAPI.Patcher.Engine.Extensions
{
	public static class FieldReplacementExtensions
	{
		/// <summary>
		/// Replaces all occurrences of a field with a property call by simply swapping
		/// the field instructions that load/set.
		/// </summary>
		/// <param name="field"></param>
		/// <param name="property"></param>
		public static void ReplaceWith(this FieldDefinition field, PropertyDefinition property)
		{
			//Enumerate over every instruction in the fields assembly
			field.Module.ForEachInstruction((method, instruction) =>
			{
				//Check if the instruction is a field reference
				//We only want to handle the field we want to replace
				var reference = instruction.Operand as FieldReference;
				if (reference != null && reference.FullName == field.FullName)
				{
					//If the instruction is being loaded, we need to replace it
					//with the property's getter method
					if (instruction.OpCode == OpCodes.Ldfld)
					{
						//A getter is required on the property
						if (property.GetMethod == null)
							throw new MissingMethodException("Property is missing getter");

						//If the property has already been added into the assembly
						//don't swap anything in it's getter
						if (method.FullName == property.GetMethod.FullName)
							return;

						//Swap the instruction to call the propertys getter
						instruction.OpCode = OpCodes.Callvirt;
						instruction.Operand = field.Module.Import(property.GetMethod);
					}
					else if (instruction.OpCode == OpCodes.Stfld)
					{
						//A setter is required on the property
						if (property.SetMethod == null)
							throw new MissingMethodException("Property is missing setter");

						//If the property has already been added into the assembly
						//don't swap anything in it's setter
						if (method.FullName == property.SetMethod.FullName)
							return;

						//Swap the instruction to call the propertys setter
						instruction.OpCode = OpCodes.Callvirt;
						instruction.Operand = field.Module.Import(property.SetMethod);
					}
				}
			});
		}
	}
}
