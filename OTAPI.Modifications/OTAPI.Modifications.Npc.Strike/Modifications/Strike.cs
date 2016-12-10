using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Npc
{
	public class Strike : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.4.3, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Npc.StrikeNPC...";

		public override void Run()
		{
			int tmpI = 0;
			bool tmpB = false;
			float tmpF = 0;
			double tmpD = 0;
			//var vanilla = this.Method(() => (new Terraria.NPC()).StrikeNPC(0, 0, 0, false, false, false));
			var vanilla = this.Type<Terraria.NPC>().Method("StrikeNPC");
			var callback = this.Method(() => OTAPI.Callbacks.Terraria.Npc.Strike(
			   null, ref tmpD, ref tmpI, ref tmpF, ref tmpI, ref tmpB, ref tmpB, ref tmpB, null
			));

			vanilla.InjectNonVoidBeginCallback(callback);
		}
	}
}