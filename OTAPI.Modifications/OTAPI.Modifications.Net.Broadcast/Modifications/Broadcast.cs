using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Net
{
	public class Broadcast : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.1.0, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking server broadcasting...";

		public override void Run()
		{
			this.Method(() => Terraria.Netplay.StartBroadCasting()).Wrap
			(
				beginCallback: this.Method(() => OTAPI.Callbacks.Terraria.Netplay.StartBroadCasting()),
				endCallback: null,
				beginIsCancellable: true,
				noEndHandling: false,
				allowCallbackInstance: false
			);

			this.Method(() => Terraria.Netplay.StopBroadCasting()).Wrap
			(
				beginCallback: this.Method(() => OTAPI.Callbacks.Terraria.Netplay.StopBroadCasting()),
				endCallback: null,
				beginIsCancellable: true,
				noEndHandling: false,
				allowCallbackInstance: false
			);
		}
	}
}