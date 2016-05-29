using Mono.Cecil;
using System.Linq;

namespace OTAPI.Patcher.Extensions
{
    public static partial class CecilHelpers
    {
        public static TypeDefinition Type(this ModuleDefinition moduleDefinition, string name)
        {
            return moduleDefinition.Types.Single(x => x.FullName == name);
        }
    }
}
