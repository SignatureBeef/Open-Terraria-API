using Mono.Cecil;
using OTAPI.Patcher.Extensions;

namespace OTAPI.Patcher.Modifications.Helpers
{
    /// <summary>
    /// Defines all types used by OTAPI patches
    /// </summary>
    public class TerrariaOrganiser
    {
        public OTAPIContext Context { get; }

        public TerrariaOrganiser(OTAPIContext context)
        {
            this.Context = context;
            this.Types = new OrganiserTypes(this);
        }

        public AssemblyDefinition Assembly => Context.Assemblies.OTAPI;

        public ModuleDefinition MainModue => Assembly.MainModule;

        public OrganiserTypes Types { get; }

        public class OrganiserTypes
        {
            internal TerrariaOrganiser _organiser;
            internal OrganiserTypes(TerrariaOrganiser organiser) { _organiser = organiser; }

            public TypeDefinition Entity => _organiser.MainModue.Type("Terraria.Entity");

            public TypeDefinition Main => _organiser.MainModue.Type("Terraria.Main");

            public TypeDefinition Program => _organiser.MainModue.Type("Terraria.Program");

            public TypeDefinition WindowsLaunch => _organiser.MainModue.Type("Terraria.WindowsLaunch");
        }
    }
}
