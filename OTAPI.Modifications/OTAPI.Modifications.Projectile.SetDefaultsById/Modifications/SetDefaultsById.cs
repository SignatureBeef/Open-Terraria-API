using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Projectile
{
	public class SetDefaultsById : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.4.1, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.3.1, Culture=neutral, PublicKeyToken=null"

		};
		public override string Description => "Hooking Projectile.SetDefaults(int)...";

		public override void Run()
		{
			var vanilla = SourceDefinition.Type("Terraria.Projectile").Methods.Single(
				x => x.Name == "SetDefaults"
				&& x.Parameters.Single().ParameterType == SourceDefinition.MainModule.TypeSystem.Int32
			);


			var cbkBegin = ModificationDefinition.Type("OTAPI.Callbacks.Terraria.Projectile").Method("SetDefaultsByIdBegin", parameters: vanilla.Parameters);
			var cbkEnd = ModificationDefinition.Type("OTAPI.Callbacks.Terraria.Projectile").Method("SetDefaultsByIdEnd", parameters: vanilla.Parameters);

			vanilla.Wrap
			(
				beginCallback: cbkBegin,
				endCallback: cbkEnd,
				beginIsCancellable: true,
				noEndHandling: false,
				allowCallbackInstance: true
			);
		}
	}
}
