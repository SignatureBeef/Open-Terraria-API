using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modifications.Helpers;
using System;

namespace OTAPI.Patcher.Modifications.Hooks.Net
{
	public class SendData : OTAPIModification<OTAPIContext>
	{
		public override string Description => "Hooking NetMessage.SendData...";

		public override void Run(OptionSet options)
		{
			var vanilla = this.Context.Terraria.Types.NetMessage.Method("SendData");
			var callback = this.Context.OTAPI.Types.NetMessage.Method("SendData");

			//Few stack issues arose trying to inject a callack before for lock, so i'll resort to 
			//wrapping the method;

			vanilla.Wrap(callback, null, true);

			Console.WriteLine("Done");
		}
	}
}