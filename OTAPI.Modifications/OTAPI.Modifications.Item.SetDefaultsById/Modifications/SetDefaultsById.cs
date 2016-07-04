using NDesk.Options;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Modifications.Hooks.Item
{
	public class SetDefaultsById : ModificationBase
	{
		public override string Description => "Hooking Item.SetDefaults(int,bool)...";
		public override void Run(OptionSet options)
		{
			var vanilla = SourceDefinition.Type("Terraria.Item").Methods.Single(
				x => x.Name == "SetDefaults"
				&& x.Parameters.First().ParameterType == this.SourceDefinition.MainModule.TypeSystem.Int32
				&& x.Parameters.Skip(1).First().ParameterType == this.SourceDefinition.MainModule.TypeSystem.Boolean
			);


			var cbkBegin = this.ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.Item").Method("SetDefaultsByIdBegin", parameters: vanilla.Parameters);
			var cbkEnd = this.ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.Item").Method("SetDefaultsByIdEnd", parameters: vanilla.Parameters);

			vanilla.Wrap(cbkBegin, cbkEnd, true);
		}
	}
}
