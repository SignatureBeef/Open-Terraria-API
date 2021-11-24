using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Collections.Generic;

namespace OTAPI.Modifications.Mono.Modifications
{
	public class WindowService : ModificationBase
	{
		public override IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.3.2, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Removing WindowService native calls...";

		public override void Run()
		{
			var type = this.SourceDefinition.Type("ReLogic.OS.Windows.WindowService");

			type.Method("SetQuickEditEnabled").ClearBody();
		}
	}
}
