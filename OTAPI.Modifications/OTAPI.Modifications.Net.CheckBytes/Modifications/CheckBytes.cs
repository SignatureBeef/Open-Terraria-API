using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

using System;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Net
{
	public class SendBytes : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking NetMessage.CheckBytes...";

		public override void Run()
		{
			var vanilla = this.Method(() => Terraria.NetMessage.CheckBytes(0));
			int bufferIndex = 0;
			var callback = vanilla.Module.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.NetMessage.CheckBytes(ref bufferIndex))
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