using Mono.Cecil;
using System.Linq;

namespace OTAPI.Patcher.Inject
{
    /// <summary>
    /// This defines the OTAPI injection context. It expects that there is an OTAPI.dll loaded (from which has been ILRepacked).
    /// </summary>
    public class OTAPIContext : InjectionContext
    {
        public OTAPIContext()
        {
            OTAPI = new OTAPIOrganiser(this);
        }

        /// <summary>
        /// OTAPI helpers
        /// </summary>
        public OTAPIOrganiser OTAPI { get; }
    }

    /// <summary>
    /// Defines all types used by OTAPI patches
    /// </summary>
    public class OTAPIOrganiser
    {
        public OTAPIContext Context { get; }

        public OTAPIOrganiser(OTAPIContext context)
        {
            this.Context = context;
        }

        public AssemblyDefinition Assembly => Context.Assemblies.OTAPI;

        public ModuleDefinition MainModue => Assembly.MainModule;

        public TypeDefinition WindowsLaunch => MainModue.Type("WindowsLaunch");
    }

    public static partial class CecilHelpers
    {
        public static TypeDefinition Type(this ModuleDefinition moduleDefinition, string name)
        {
            return moduleDefinition.Types.Single(x => x.Name == name);
        }

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
