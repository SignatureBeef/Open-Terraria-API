using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Npc
{
	public class SetDefaultsByName : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.4.3, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.4.3, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Npc.SetDefaults(string)...";
		public override void Run()
		{
			var vanilla = this.Method(() => (new Terraria.NPC()).SetDefaults(""));

			string tmp = null;
			var cbkBegin = this.Method(() => OTAPI.Callbacks.Terraria.Npc.SetDefaultsByNameBegin(null, ref tmp));
			var cbkEnd = this.Method(() => OTAPI.Callbacks.Terraria.Npc.SetDefaultsByNameEnd(null, ref tmp));

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
