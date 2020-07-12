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

namespace ModFramework
{
    [MonoMod.MonoModIgnore]
    public static class ReplacementExtensions
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
    }
}