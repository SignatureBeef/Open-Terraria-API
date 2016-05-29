using Mono.Cecil;
using System.Linq;

namespace OTAPI.Patcher.Extensions
{
    public static partial class CecilHelpers
    {
        public static MethodDefinition Method(this TypeDefinition typeDefinition, string name)
        {
            return typeDefinition.Methods.Single(x => x.Name == name);
        }

        public static FieldDefinition Field(this TypeDefinition typeDefinition, string name)
        {
            return typeDefinition.Fields.Single(x => x.Name == name);
        }
    }
}
