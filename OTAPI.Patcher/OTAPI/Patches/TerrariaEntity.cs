using NDesk.Options;
using OTAPI.Patcher.Modification;
using OTAPI.Patcher.Modifications.Helpers;
using System;

namespace OTAPI.Patcher.Modifications.Patches
{
    /// <summary>
    /// This patch is to inject our TerrariaEntity so it can be used as the base type of all Terraria entities.
    /// Doing this will allow the terraria entities to join the OTAPI IEntity network.
    /// </summary>
    public class TerrariaEntity : OTAPIModification<OTAPIContext>
    {
        public override void Run(OptionSet options)
        {
            Console.Write("Injecting TerrariaEntity...");

            this.Context.Terraria.Types.Entity.BaseType = this.Context.OTAPI.Types.TerrariaEntity;

            Console.WriteLine("Done");
        }
    }
}
