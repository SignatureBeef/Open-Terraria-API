using NDesk.Options;
using OTAPI.Patcher.Engine.Modification;
using OTAPI.Patcher.Engine.Modifications.Helpers;
using System;

namespace OTAPI.Patcher.Engine.Modifications.Patches
{
    /// <summary>
    /// This patch is to inject our TerrariaEntity so it can be used as the base type of all Terraria entities.
    /// Doing this will allow the terraria entities to join the OTAPI IEntity network.
    /// </summary>
    public class TerrariaEntity : OTAPIModification<OTAPIContext>
    {
		public override string Description =>  "Injecting TerrariaEntity...";
        public override void Run()
        {
            this.Context.Terraria.Types.Entity.BaseType = this.Context.OTAPI.Types.TerrariaEntity;
        }
    }
}
