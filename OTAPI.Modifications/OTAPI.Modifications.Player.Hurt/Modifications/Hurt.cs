using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Player
{
	public class Hurt : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"Terraria, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Player.Hurt";
		public override void Run()
		{
			var vanilla = this.Method(() => (new Terraria.Player()).Hurt(null, 0, 0, false, false, false, 0));

			int tmp = 0;
			bool tmp1 = false;
			double tmp3 = 0;
			var cbkBegin = //this.SourceDefinition.MainModule.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.Player.HurtBegin(null, ref tmp3, null, ref tmp, ref tmp, ref tmp1, ref tmp1, ref tmp1, ref tmp));
			//);

			vanilla.InjectNonVoidBeginCallback(cbkBegin);
		}
	}
}
