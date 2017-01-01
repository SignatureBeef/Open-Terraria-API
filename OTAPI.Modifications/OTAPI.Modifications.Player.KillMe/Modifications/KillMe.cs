using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Player
{
	public class KillMe : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"Terraria, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Player.KillMe";
		public override void Run()
		{
			var vanilla = this.Method(() => (new Terraria.Player()).KillMe(null, 0, 0, false));

			double tmp = 0;
			int tmp1 = 0;
			bool tmp2 = false;
			Terraria.DataStructures.PlayerDeathReason tmpDs = null;
			var cbkBegin = this.SourceDefinition.MainModule.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.Player.KillMeBegin(null, null, ref tmp, ref tmp1, ref tmp2))
			);

			var cbkEnd = this.SourceDefinition.MainModule.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.Player.KillMeEnd(null, null, 0, 0, false))
			);

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
