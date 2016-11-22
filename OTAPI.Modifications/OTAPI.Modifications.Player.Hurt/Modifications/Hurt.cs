using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Player
{
	public class Hurt : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"Terraria, Version=1.3.4.3, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Player.Hurt";
		public override void Run()
		{
			var vanilla = this.Method(() => (new Terraria.Player()).Hurt(null, 0, 0, false, false, false, 0));

			int tmp = 0;
			bool tmp1 = false;
			string tmp2 = null;
			double tmp3 = 0;
			Terraria.DataStructures.PlayerDeathReason   tmp4 = null;
			var cbkBegin = //this.SourceDefinition.MainModule.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.Player.HurtBegin(ref tmp3, null, ref tmp4, ref tmp, ref tmp, ref tmp1, ref tmp1, ref tmp2, ref tmp1, ref tmp));
			//);

			vanilla.InjectNonVoidBeginCallback(cbkBegin);
		}
	}
}
