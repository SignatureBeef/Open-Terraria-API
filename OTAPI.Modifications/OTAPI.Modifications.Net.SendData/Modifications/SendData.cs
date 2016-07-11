using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

using System;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Net
{
	public class SendData : ModificationBase
	{
		public override string Description => "Hooking NetMessage.SendData...";

		public override void Run()
		{
			var vanilla = SourceDefinition.Type("Terraria.NetMessage").Method("SendData");
			var callback = ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.NetMessage").Method("SendData");

			//Few stack issues arose trying to inject a callback before for lock, so i'll resort to 
			//wrapping the method;
			
			vanilla.Wrap
			(
				beginCallback: callback,
				endCallback: null,
				beginIsCancellable: true,
				noEndHandling: false,
				allowCallbackInstance: false
			);

			Console.WriteLine("Done");
		}
	}
}