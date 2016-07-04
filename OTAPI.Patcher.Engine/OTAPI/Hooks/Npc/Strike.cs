using NDesk.Options;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Npc
{
	public class Strike : ModificationBase
	{
		public override string Description => "Hooking Npc.StrikeNPC...";

		public override void Run(OptionSet options)
		{
			var vanilla = SourceDefinition.Type("Terraria.NPC").Method("StrikeNPC");
			var callback = ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.Npc").Method("Strike");

			vanilla.InjectNonVoidCallback(callback);
		}
	}
}
