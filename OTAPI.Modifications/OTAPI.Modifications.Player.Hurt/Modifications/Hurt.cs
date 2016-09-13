using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Player
{
	public class Hurt : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"Terraria, Version=1.3.3.2, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Player.Hurt";
		public override void Run()
		{
			var vanilla = SourceDefinition.Type("Terraria.Player")
				.Method("Hurt");

			int tmp = 0;
			bool tmp1 = false;
			string tmp2 = null;
			double tmp3 = 0;
			var cbkBegin = //this.SourceDefinition.MainModule.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.Player.HurtBegin(ref tmp3, null, ref tmp, ref tmp, ref tmp1, ref tmp1, ref tmp2, ref tmp1, ref tmp));
			//);

			vanilla.InjectNonVoidBeginCallback(cbkBegin);
		}
	}
}
