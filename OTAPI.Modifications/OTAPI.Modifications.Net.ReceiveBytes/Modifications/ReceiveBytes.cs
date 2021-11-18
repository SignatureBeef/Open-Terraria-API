using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Net
{
	public class ReceiveBytes : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.3.0, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking NetMessage.ReceiveBytes...";

		public override void Run()
		{
			var vanilla = this.Method(() => Terraria.NetMessage.ReceiveBytes(new byte[0], 0, 256));
			byte[] bytes = new byte[0];
			int streamLength = 0;
			int bufferIndex = 0;
			var callback = vanilla.Module.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.NetMessage.ReceiveBytes(ref bytes, ref streamLength, ref bufferIndex))
			);

			vanilla.Wrap
			(
				beginCallback: callback,
				endCallback: null,
				beginIsCancellable: true,
				noEndHandling: false,
				allowCallbackInstance: false
			);
		}
	}
}
