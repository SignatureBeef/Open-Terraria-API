using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Npc
{
	public class Strike : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.2.1, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.2.1, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Npc.StrikeNPC...";

		public override void Run()
		{
			var vanilla = SourceDefinition.Type("Terraria.NPC").Method("StrikeNPC");
			var callback = ModificationDefinition.Type("OTAPI.Callbacks.Terraria.Npc").Method("Strike");

			vanilla.InjectNonVoidBeginCallback(callback);
		}
	}
}