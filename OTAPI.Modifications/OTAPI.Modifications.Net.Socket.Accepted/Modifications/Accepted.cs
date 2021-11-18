using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Net.Socket
{
	public class ServerSocketAccepted : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.3.0, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Netplay.OnConnectionAccepted...";

		public override void Run()
		{
			//Get the private vanilla reference
			var vanilla = SourceDefinition.Type("Terraria.Netplay").Method("OnConnectionAccepted");
			var callback = SourceDefinition.MainModule.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.Netplay.OnConnectionAccepted(null))
			);
			
			vanilla.Wrap
			(
				beginCallback: callback,
				endCallback: null,
				beginIsCancellable: true,
				noEndHandling: true,
				allowCallbackInstance: false
			);
		}
	}
}
