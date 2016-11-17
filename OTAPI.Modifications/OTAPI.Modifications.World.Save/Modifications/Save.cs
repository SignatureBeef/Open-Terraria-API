using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.World.IO
{
	public class Save : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.4.2, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking WorldFile.saveWorld(bool,bool)...";
		public override void Run()
		{
			var vanilla = SourceDefinition.Type("Terraria.IO.WorldFile").Methods.Single(
				x => x.Name == "saveWorld"
				&& x.Parameters.Count() == 2
				&& x.Parameters[0].ParameterType == SourceDefinition.MainModule.TypeSystem.Boolean
				&& x.Parameters[1].ParameterType == SourceDefinition.MainModule.TypeSystem.Boolean
			);

			var cbkBegin = ModificationDefinition.Type("OTAPI.Callbacks.Terraria.WorldFile").Method("SaveWorldBegin", parameters: vanilla.Parameters);
			var cbkEnd = ModificationDefinition.Type("OTAPI.Callbacks.Terraria.WorldFile").Method("SaveWorldEnd", parameters: vanilla.Parameters);

			vanilla.Wrap
			(
				beginCallback: cbkBegin,
				endCallback: cbkEnd,
				beginIsCancellable: true,
				noEndHandling: false,
				allowCallbackInstance: false
			);
		}
	}
}
