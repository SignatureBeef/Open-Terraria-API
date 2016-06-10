﻿using Mono.Cecil;
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
            
            public TypeDefinition Collision => _organiser.MainModue.Type("OTAPI.Core.Callbacks.Terraria.Collision");

            public TypeDefinition Item => _organiser.MainModue.Type("OTAPI.Core.Callbacks.Terraria.Item");

            public TypeDefinition Main => _organiser.MainModue.Type("OTAPI.Core.Callbacks.Terraria.Main");
            
            public TypeDefinition MessageBuffer => _organiser.MainModue.Type("OTAPI.Core.Callbacks.Terraria.MessageBuffer");

            public TypeDefinition NetMessage => _organiser.MainModue.Type("OTAPI.Core.Callbacks.Terraria.NetMessage");

            public TypeDefinition Npc => _organiser.MainModue.Type("OTAPI.Core.Callbacks.Terraria.Npc");
            
            public TypeDefinition Projectile => _organiser.MainModue.Type("OTAPI.Core.Callbacks.Terraria.Projectile");

            public TypeDefinition TerrariaEntity => _organiser.MainModue.Type("OTAPI.Core.TerrariaEntity");
            
            public TypeDefinition WorldFile => _organiser.MainModue.Type("OTAPI.Core.Callbacks.Terraria.WorldFile");
            
            public TypeDefinition WorldGen => _organiser.MainModue.Type("OTAPI.Core.Callbacks.Terraria.WorldGen");
        }
    }
}