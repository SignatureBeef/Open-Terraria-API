using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;
using MonoMod.Utils;

namespace OTAPI
{
    public static class InterfaceEmitter
    {
        public static TypeDefinition RemapWithInterface(this TypeDefinition ElementType)
        {
            TypeDefinition @interface = new TypeDefinition(
                ElementType.Namespace,
                $"I{ElementType.Name}",
                TypeAttributes.Abstract | TypeAttributes.ClassSemanticMask | TypeAttributes.Public
            );

            foreach (var field in ElementType.Fields.Where(f => !f.HasConstant && !f.IsPrivate))
            {
                var cf = field.Clone();
                @interface.Fields.Add(cf);
            }

            foreach (var prop in ElementType.Properties)
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