using NDesk.Options;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Projectile
{
	public class Update : ModificationBase
	{
		public override string Description => "Hooking Projectile.Update(int)...";
		public override void Run(OptionSet options)
		{
			var vanilla = SourceDefinition.Type("Terraria.Projectile").Methods.Single(
				x => x.Name == "Update"
				&& x.Parameters.Single().ParameterType == SourceDefinition.MainModule.TypeSystem.Int32
			);

			var cbkBegin = ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.Projectile").Method("UpdateBegin", parameters: vanilla.Parameters);
			var cbkEnd = ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.Projectile").Method("UpdateEnd", parameters: vanilla.Parameters);

			vanilla.Wrap(cbkBegin, cbkEnd, true);
		}
	}
}
