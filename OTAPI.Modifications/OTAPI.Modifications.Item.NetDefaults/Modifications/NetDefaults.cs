using NDesk.Options;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Modifications.Hooks.Item
{
	public class NetDefaults : ModificationBase
    {
		public override string Description => "Hooking Item.NetDefaults(int)...";
        public override void Run(OptionSet options)
        {
            var vanilla = this.SourceDefinition.Type("Terraria.Item").Methods.Single(
                x => x.Name == "netDefaults"
                && x.Parameters.First().ParameterType == this.SourceDefinition.MainModule.TypeSystem.Int32
            );


            var cbkBegin = this.ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.Item").Method("NetDefaultsBegin", parameters: vanilla.Parameters);
            var cbkEnd = this.ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.Item").Method("NetDefaultsEnd", parameters: vanilla.Parameters);

            vanilla.Wrap(cbkBegin, cbkEnd, true);
        }
    }
}
