using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Projectile
{
	public class AI : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.5.0, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null"

		};
		public override string Description => "Hooking Projectile.AI()...";
		public override void Run()
		{
			var vanilla = this.Method(() => (new Terraria.Projectile()).AI());

			var cbkBegin = this.Method(() => OTAPI.Callbacks.Terraria.Projectile.AIBegin(null));
			var cbkEnd = this.Method(() => OTAPI.Callbacks.Terraria.Projectile.AIEnd(null));

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
