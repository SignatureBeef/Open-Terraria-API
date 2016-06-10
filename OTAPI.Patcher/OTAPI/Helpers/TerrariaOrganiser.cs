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

        public AssemblyDefinition Assembly => Context.Assemblies.Terraria;

        public ModuleDefinition MainModue => Assembly.MainModule;

        public OrganiserTypes Types { get; }

        public class OrganiserTypes
        {
            internal TerrariaOrganiser _organiser;
            internal OrganiserTypes(TerrariaOrganiser organiser) { _organiser = organiser; }
            
            public TypeDefinition Collision => _organiser.MainModue.Type("Terraria.Collision");

            public TypeDefinition Entity => _organiser.MainModue.Type("Terraria.Entity");

            public TypeDefinition Item => _organiser.MainModue.Type("Terraria.Item");

            public TypeDefinition Main => _organiser.MainModue.Type("Terraria.Main");

            public TypeDefinition MessageBuffer => _organiser.MainModue.Type("Terraria.MessageBuffer");

            public TypeDefinition NetMessage => _organiser.MainModue.Type("Terraria.NetMessage");

            public TypeDefinition Npc => _organiser.MainModue.Type("Terraria.NPC");

            public TypeDefinition Program => _organiser.MainModue.Type("Terraria.Program");
            
            public TypeDefinition Projectile => _organiser.MainModue.Type("Terraria.Projectile");

            public TypeDefinition WindowsLaunch => _organiser.MainModue.Type("Terraria.WindowsLaunch");

            public TypeDefinition WorldFile => _organiser.MainModue.Type("Terraria.IO.WorldFile");
            
            public TypeDefinition WorldGen => _organiser.MainModue.Type("Terraria.WorldGen");
        }
    }
}
