using NDesk.Options;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modifications.Helpers;
using System;
using System.Linq;

namespace OTAPI.Patcher.Modifications.Hooks.Item
{
    public class NetDefaults : OTAPIModification<OTAPIContext>
    {
		public override string Description => "Hooking Item.NetDefaults(int)...";
        public override void Run(OptionSet options)
        {
            var vanilla = this.Context.Terraria.Types.Item.Methods.Single(
                x => x.Name == "netDefaults"
                && x.Parameters.First().ParameterType == this.Context.Terraria.MainModue.TypeSystem.Int32
            );


            var cbkBegin = this.Context.OTAPI.Types.Item.Method("NetDefaultsBegin", parameters: vanilla.Parameters);
            var cbkEnd = this.Context.OTAPI.Types.Item.Method("NetDefaultsEnd", parameters: vanilla.Parameters);

            vanilla.Wrap(cbkBegin, cbkEnd, true);
        }
    }
}
