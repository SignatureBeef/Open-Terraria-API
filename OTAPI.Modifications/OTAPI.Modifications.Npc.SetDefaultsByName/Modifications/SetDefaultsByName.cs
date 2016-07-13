using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Npc
{
	public class SetDefaultsByName : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.1.1, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Npc.SetDefaults(string)...";
		public override void Run()
		{
			var vanilla = SourceDefinition.Type("Terraria.NPC").Methods.Single(
				x => x.Name == "SetDefaults"
				&& x.Parameters.First().ParameterType == SourceDefinition.MainModule.TypeSystem.String
			);

			var cbkBegin = ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.Npc").Method("SetDefaultsByNameBegin", parameters: vanilla.Parameters);
			var cbkEnd = ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.Npc").Method("SetDefaultsByNameEnd", parameters: vanilla.Parameters);

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
