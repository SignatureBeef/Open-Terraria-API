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
using ModFramework.Relinker;
using System.Linq;

namespace ModFramework
{
    [MonoMod.MonoModIgnore]
    public static class InterfaceEmitter
    {
        public static TypeDefinition RemapAsInterface(this TypeDefinition ElementType, IRelinkProvider relinkProvider)
        {
            var iitem = ElementType.RemapWithInterface();

            relinkProvider.AddTask(new InterfaceRelinker(ElementType, iitem));

            return iitem;
        }

        public static TypeDefinition RemapWithInterface(this TypeDefinition ElementType)
        {
            TypeDefinition @interface = new TypeDefinition(
                ElementType.Namespace,
                $"I{ElementType.Name}",
                TypeAttributes.Abstract | TypeAttributes.ClassSemanticMask | TypeAttributes.Public
            );

            if (ElementType.Fields.Any(f => !f.HasConstant && !f.IsPrivate))
                throw new System.NotSupportedException("CS0525: Interfaces cannot contain instance fields");
            // foreach (var field in ElementType.Fields.Where(f => !f.HasConstant && !f.IsPrivate))
            // {
            //     var cf = field.Clone();
            //     @interface.Fields.Add(cf);
            // }

            foreach (var prop in ElementType.Properties.Where(p => p.HasThis))
            {
                var cp = prop.Clone();
                cp.DeclaringType = null;
                foreach (var method in new[] { cp.GetMethod, cp.SetMethod }.Where(x => x != null))
                {
                    method.DeclaringType = null;
                    method.Body = null;

                    // enforce interface requirements
                    method.Attributes |= MethodAttributes.NewSlot | MethodAttributes.Abstract | MethodAttributes.Virtual;

                    // remove any System.Runtime.CompilerServices.CompilerGeneratedAttribute
                    var attr = method.CustomAttributes.SingleOrDefault(x =>
                        x.AttributeType.FullName == "System.Runtime.CompilerServices.CompilerGeneratedAttribute"
                    );
                    if (attr != null)
                        method.CustomAttributes.Remove(attr);

                    @interface.Methods.Add(method);
                }
                @interface.Properties.Add(cp);

                // satisfy the interface by marking the properties as implemented
                if (prop.GetMethod != null)
                    prop.GetMethod.IsNewSlot = prop.GetMethod.IsFinal = prop.GetMethod.IsVirtual = true;
                if (prop.SetMethod != null)
                    prop.SetMethod.IsNewSlot = prop.SetMethod.IsFinal = prop.SetMethod.IsVirtual = true;
            }

            foreach (var method in ElementType.Methods
                .Where(m => (m.IsPublic || m.IsAssembly)
                    && !m.IsStatic
                    && !m.IsConstructor
                    && !m.IsVirtual
                    && !m.IsGetter
                    && !m.IsSetter
                )
            )
            {
                var cm = method.Clone();
                cm.DeclaringType = null;
                cm.Body = null;
                // enforce interface requirements
                cm.Attributes |= MethodAttributes.NewSlot | MethodAttributes.Abstract | MethodAttributes.Virtual;
                @interface.Methods.Add(cm);

                // satisfy the interface by marking the properties as implemented
                method.IsNewSlot = method.IsFinal = method.IsVirtual = true;
            }

            ElementType.Module.Types.Add(@interface);

            ElementType.Interfaces.Add(new InterfaceImplementation(@interface));

            return @interface;
        }
    }
}