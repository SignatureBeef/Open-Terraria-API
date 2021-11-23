using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.World
{
	public class SpawnNpc : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.3.1, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking world npc spawning...";
		public override void Run()
		{
			var vanilla = this.Method(() => Terraria.NPC.SpawnNPC());
			var callback = this.Method(() => OTAPI.Callbacks.Terraria.World.SpawnNpc ());

			vanilla.Wrap
			(
				beginCallback: callback,
				endCallback: null,
				beginIsCancellable: true,
				noEndHandling: false,
				allowCallbackInstance: false
			);
		}
	}
}
