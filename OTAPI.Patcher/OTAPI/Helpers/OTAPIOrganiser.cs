using Mono.Cecil;
using OTAPI.Patcher.Extensions;

namespace OTAPI.Patcher.Modifications.Helpers
{
    /// <summary>
    /// Defines all types used by OTAPI patches
    /// </summary>
    public class OTAPIOrganiser
    {
        public OTAPIContext Context { get; }

        public OTAPIOrganiser(OTAPIContext context)
        {
            this.Context = context;
            this.Types = new OrganiserTypes(this);
        }

        public AssemblyDefinition Assembly => Context.Assemblies.OTAPI;

        public ModuleDefinition MainModue => Assembly.MainModule;

        public OrganiserTypes Types { get; }

        public class OrganiserTypes
        {
            internal OTAPIOrganiser _organiser;
            internal OrganiserTypes(OTAPIOrganiser organiser) { _organiser = organiser; }
            
            public TypeDefinition Item => _organiser.MainModue.Type("OTAPI.Core.Callbacks.Terraria.Item");

            public TypeDefinition Main => _organiser.MainModue.Type("OTAPI.Core.Callbacks.Terraria.Main");
            
            public TypeDefinition Npc => _organiser.MainModue.Type("OTAPI.Core.Callbacks.Terraria.Npc");

            public TypeDefinition TerrariaEntity => _organiser.MainModue.Type("OTAPI.Core.TerrariaEntity");
        }
    }
}
