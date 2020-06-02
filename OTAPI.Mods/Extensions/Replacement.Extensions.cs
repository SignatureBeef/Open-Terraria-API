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
using MonoMod.Utils;

namespace OTAPI
{
    public static partial class Extensions
    {
        public static FieldDefinition Clone(this FieldDefinition field)
        {
            return new FieldDefinition(field.Name, field.Attributes, field.FieldType);
        }

        public static PropertyDefinition Clone(this PropertyDefinition property)
        {
            var prop = new PropertyDefinition(property.Name, property.Attributes, property.PropertyType);

            // prop.HasThis = property.HasThis;
            if (property.GetMethod != null)
            {
                prop.GetMethod = property.GetMethod.Clone();
            }
            if (property.SetMethod != null)
            {
                prop.SetMethod = property.SetMethod.Clone();
            }

            return prop;
        }

        // public static string GetBackingName(this FieldDefinition field)
        // {
        // 	return $"<{field.Name}>k__BackingField";
        // }

        // public static PropertyDefinition AsProperty(this FieldDefinition field)
        // {
        // 	var emitter = new PropertyEmitter(field.Name, field.FieldType, field.DeclaringType,
        // 		getterAttributes: MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.SpecialName,
        // 		setterAttributes: MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.SpecialName
        // 	);

        // 	field.Name = field.GetBackingName();
        // 	field.IsPublic = false;
        // 	field.IsPrivate = true;

        // 	//field.CustomAttributes.Add(new CustomAttribute(
        // 	//	field.DeclaringType.Module.ImportReference(
        // 	//		typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute)
        // 	//			.GetConstructors()
        // 	//			.Single()
        // 	//	)
        // 	//));

        // 	return emitter.Emit();
        // }

        // public static PropertyDefinition AsVirtual(this PropertyDefinition property)
        // {
        // 	if (property.GetMethod != null)
        // 		property.GetMethod.IsVirtual = property.GetMethod.IsNewSlot = true;
        // 	if (property.SetMethod != null)
        // 		property.SetMethod.IsVirtual = property.SetMethod.IsNewSlot = true;

        // 	return property;
        // }
    }
}