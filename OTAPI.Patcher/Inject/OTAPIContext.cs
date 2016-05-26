using Mono.Cecil;

namespace OTAPI.Patcher.Inject
{
    public class OTAPIContext : InjectionContext
    {
        public AssemblyDefinition Terraria
        { get { return Assemblies.Terraria; } }

        public AssemblyDefinition OTAPI
        { get { return Assemblies.OTAPI; } }
    }
}
