using Mono.Cecil;
using System.Linq;
using MonoMod.Utils;

namespace OTAPI
{
    public static class InterfaceEmitter
    {
        public static TypeDefinition GenerateInterface(this TypeDefinition ElementType)
        {
            TypeDefinition collection = new TypeDefinition(
                ElementType.Namespace,
                $"I{ElementType.Name}",
                TypeAttributes.Abstract | TypeAttributes.ClassSemanticMask | TypeAttributes.Public
            );

            foreach(var field in ElementType.Fields.Where(f => !f.HasConstant))
            {
                var cf = field.Clone();
                collection.Fields.Add(cf);
            }

            foreach (var prop in ElementType.Properties)
            {
                var cp = prop.Clone();
                cp.DeclaringType = null;
                foreach (var method in new[] { cp.GetMethod, cp.SetMethod }.Where(x => x != null))
                {
                    method.DeclaringType = null;
                    method.Body = null;
                    collection.Methods.Add(method);
                }
                collection.Properties.Add(cp);
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
                collection.Methods.Add(cm);
            }

            return collection;
        }
    }
}