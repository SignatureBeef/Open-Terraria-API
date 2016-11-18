using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Npc
{
	public class NetDefaults : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.4.3, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.4.1, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Npc.NetDefaults(int)...";
		public override void Run()
		{
			var vanilla = SourceDefinition.Type("Terraria.NPC").Methods.Single(
				x => x.Name == "netDefaults"
				&& x.Parameters.First().ParameterType == SourceDefinition.MainModule.TypeSystem.Int32
			);


			var cbkBegin = ModificationDefinition.Type("OTAPI.Callbacks.Terraria.Npc").Method("NetDefaultsBegin", parameters: vanilla.Parameters);
			var cbkEnd = ModificationDefinition.Type("OTAPI.Callbacks.Terraria.Npc").Method("NetDefaultsEnd", parameters: vanilla.Parameters);

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
