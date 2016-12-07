using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Npc
{
	public class SetDefaultsById : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.4.3, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Npc.SetDefaults(int,float)...";
		public override void Run()
		{
			var vanilla = this.Method(() => (new Terraria.NPC()).SetDefaults(0, -1f));

			int tmpI = 0;
			float tmpF = 0;
			var cbkBegin = this.Method(() => OTAPI.Callbacks.Terraria.Npc.SetDefaultsByIdBegin(null, ref tmpI, ref tmpF));
			var cbkEnd = this.Method(() => OTAPI.Callbacks.Terraria.Npc.SetDefaultsByIdEnd(null, 0, 0));

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
