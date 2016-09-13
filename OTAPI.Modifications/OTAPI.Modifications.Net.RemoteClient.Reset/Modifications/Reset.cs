using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Net.RemoteClient
{
	public class RemoteClientReset : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.3.2, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking RemoteClient.Reset...";
		
		public override void Run()
		{
			var vanilla = SourceDefinition.Type("Terraria.RemoteClient").Method("Reset");

			var cbkBegin = this.Method(() => OTAPI.Callbacks.Terraria.RemoteClient.PreReset(null));
			var cbkEnd = this.Method(() => OTAPI.Callbacks.Terraria.RemoteClient.PostReset(null));

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