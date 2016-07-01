using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modifications.Helpers;
using System;
using System.Linq;

namespace OTAPI.Patcher.Modifications.Hooks.Npc
{
    public class NetDefaults : OTAPIModification<OTAPIContext>
    {
		public override string Description => "Hooking Npc.NetDefaults(int)...";
        public override void Run(OptionSet options)
        {
            var vanilla = this.Context.Terraria.Types.Npc.Methods.Single(
                x => x.Name == "netDefaults"
                && x.Parameters.First().ParameterType == this.Context.Terraria.MainModue.TypeSystem.Int32
            );


            var cbkBegin = this.Context.OTAPI.Types.Npc.Method("NetDefaultsBegin", parameters: vanilla.Parameters);
            var cbkEnd = this.Context.OTAPI.Types.Npc.Method("NetDefaultsEnd", parameters: vanilla.Parameters);

            vanilla.Wrap(cbkBegin, cbkEnd, true);
        }
    }
}
