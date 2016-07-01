using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modifications.Helpers;
using System;
using System.Linq;

namespace OTAPI.Patcher.Modifications.Hooks.Item
{
    public class SetDefaultsById : OTAPIModification<OTAPIContext>
    {
		public override string Description => "Hooking Item.SetDefaults(int,bool)...";
        public override void Run(OptionSet options)
        {
            var vanilla = this.Context.Terraria.Types.Item.Methods.Single(
                x => x.Name == "SetDefaults"
                && x.Parameters.First().ParameterType == this.Context.Terraria.MainModue.TypeSystem.Int32
                && x.Parameters.Skip(1).First().ParameterType == this.Context.Terraria.MainModue.TypeSystem.Boolean
            );


            var cbkBegin = this.Context.OTAPI.Types.Item.Method("SetDefaultsByIdBegin", parameters: vanilla.Parameters);
            var cbkEnd = this.Context.OTAPI.Types.Item.Method("SetDefaultsByIdEnd", parameters: vanilla.Parameters);

            vanilla.Wrap(cbkBegin, cbkEnd, true);
        }
    }
}
