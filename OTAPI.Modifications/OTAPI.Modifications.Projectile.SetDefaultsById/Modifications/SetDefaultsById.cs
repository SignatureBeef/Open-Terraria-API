using NDesk.Options;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

using System;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Projectile
{
	public class SetDefaultsById : ModificationBase
	{
		public override string Description => "Hooking Projectile.SetDefaults(int)...";

        public override void Run()
        {
			var vanilla = SourceDefinition.Type("Terraria.Projectile").Methods.Single(
				x => x.Name == "SetDefaults"
				&& x.Parameters.Single().ParameterType == SourceDefinition.MainModule.TypeSystem.Int32
			);


			var cbkBegin = ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.Projectile").Method("SetDefaultsByIdBegin", parameters: vanilla.Parameters);
			var cbkEnd = ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.Projectile").Method("SetDefaultsByIdEnd", parameters: vanilla.Parameters);

			vanilla.Wrap(cbkBegin, cbkEnd, true);
		}
	}
}
