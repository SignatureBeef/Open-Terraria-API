using Microsoft.Xna.Framework;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTAPI.Modifications.NetworkText.Modifications
{
	public class SendChatMessageToClientModification : ModificationBase
	{
		public override IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.5.1, Culture=neutral, PublicKeyToken=null",
		};

		public override string Description => "Hooking Terraria.NetMessage.SendChatMessageToClient";

		public override void Run()
		{
			Color tmpColor = new Color();
			int tmpInt = 0;
			var broadcastChatMessage = Method<Terraria.NetMessage>("SendChatMessageToClient", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
			var broadcastChatMessageBeforeCallback = Method(() => Callbacks.BroadcastChatMessageCallback.BeforeBroadcastChatMessage(null, ref tmpColor, ref tmpInt));
			var broadcastChatMessageAfterCallback = Method(() => Callbacks.BroadcastChatMessageCallback.AfterBroadcastChatMessage(null, ref tmpColor, ref tmpInt));

			broadcastChatMessage.Wrap(broadcastChatMessageBeforeCallback, 
				broadcastChatMessageAfterCallback,
				beginIsCancellable: true,
				noEndHandling: true,
				allowCallbackInstance: false);

		}
	}
}
