using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Collections.Generic;

namespace OTAPI.Modifications.Mono.Modifications
{
	public class Sleep : ModificationBase
	{
		public override IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.1.2, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Removing windows specific thread calls...";

		public override void Run()
		{
			var NeverSleep = this.SourceDefinition.Type("Terraria.Main").Method("NeverSleep");
			var YouCanSleepNow = this.SourceDefinition.Type("Terraria.Main").Method("YouCanSleepNow");

			NeverSleep.ClearBody();
			YouCanSleepNow.ClearBody();
		}
	}
}
