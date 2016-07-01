using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modifications.Helpers;
using System;
using System.Linq;

namespace OTAPI.Patcher.Modifications.Hooks.World
{
    public class Hardmode : OTAPIModification<OTAPIContext>
    {
		public override string Description => "Hooking WorldGen.StartHardmode()...";
        public override void Run(OptionSet options)
        {
            var vanilla = this.Context.Terraria.Types.WorldGen.Methods.Single(
                x => x.Name == "StartHardmode"
                && x.Parameters.Count() == 0
            );


            var cbkBegin = this.Context.OTAPI.Types.WorldGen.Method("HardmodeBegin", parameters: vanilla.Parameters);
            var cbkEnd = this.Context.OTAPI.Types.WorldGen.Method("HardmodeEnd", parameters: vanilla.Parameters);

            vanilla.Wrap(cbkBegin, cbkEnd, true);
        }
    }
}
