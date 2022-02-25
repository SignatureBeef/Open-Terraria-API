using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Collections.Generic;

namespace OTAPI.Modifications
{
	public class PlatformModification : ModificationBase
	{
		public override IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.3.4, Culture=neutral, PublicKeyToken=null"
		};

		public override string Description => "Modifying readonly flag on ReLogic.OS.Platform.Current";

		public override void Run()
		{
			this.SourceDefinition.Type("ReLogic.OS.Platform").Field("Current").IsInitOnly = false;
		}
	}
}
