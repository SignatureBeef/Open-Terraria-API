using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Projectile
{
	public class Kill : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.4.3, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.4.3, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Projectile.Kill()...";

		public override void Run()
		{
			var vanilla = this.Method(() => (new Terraria.Projectile()).Kill());

			var cbkBegin = this.Method(() => Callbacks.Terraria.Projectile.KillBegin(null));
			var cbkEnd = this.Method(() => Callbacks.Terraria.Projectile.KillEnd(null));

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
