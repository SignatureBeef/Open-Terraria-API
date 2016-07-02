using NDesk.Options;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modifications.Helpers;
using System;
using System.Linq;

namespace OTAPI.Patcher.Modifications.Hooks.Item
{
    public class SetDefaultsByName : OTAPIModification<OTAPIContext>
    {
		public override string Description => "Hooking Item.SetDefaults(string)...";
        public override void Run(OptionSet options)
        {
            var vanilla = this.Context.Terraria.Types.Item.Methods.Single(
                x => x.Name == "SetDefaults"
                && x.Parameters.First().ParameterType == this.Context.Terraria.MainModue.TypeSystem.String
            );
            
            var cbkBegin = this.Context.OTAPI.Types.Item.Method("SetDefaultsByNameBegin", parameters: vanilla.Parameters);
            var cbkEnd = this.Context.OTAPI.Types.Item.Method("SetDefaultsByNameEnd", parameters: vanilla.Parameters);

            vanilla.Wrap(cbkBegin, cbkEnd, true);
        }
    }
}
