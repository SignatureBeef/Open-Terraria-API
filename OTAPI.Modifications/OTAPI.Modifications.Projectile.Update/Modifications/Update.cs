using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Projectile
{
	public class Update : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.2.1, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Projectile.Update(int)...";
		public override void Run()
		{
			var vanilla = this.Method(() => (new Terraria.Projectile()).Update(0));

			int tmp = 0;
			var cbkBegin = this.Method(() => OTAPI.Callbacks.Terraria.Projectile.UpdateBegin(null, ref tmp));
			var cbkEnd = this.Method(() => OTAPI.Callbacks.Terraria.Projectile.UpdateEnd(null, 0));

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
