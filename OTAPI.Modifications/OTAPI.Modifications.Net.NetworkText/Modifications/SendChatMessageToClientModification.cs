using Microsoft.Xna.Framework;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Collections.Generic;

namespace OTAPI.Modifications.NetworkText.Modifications
{
	public class SendChatMessageToClientModification : ModificationBase
	{
		public override IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.3.2, Culture=neutral, PublicKeyToken=null",
		};

		public override string Description => "Hooking Terraria.Chat.ChatHelper.SendChatMessageToClient";

		public override void Run()
		{
			Color tmpColor = new Color();
			int tmpInt = 0;
			var chatHelper = this.SourceDefinition.Type("Terraria.Chat.ChatHelper");
			var sendChatMessage = chatHelper.Method("SendChatMessageToClient");
			var sendChatMessageBeforeCallback = Method(() => Callbacks.Terraria.NetMessage.BeforeSendChatMessageToClient(null, ref tmpColor, ref tmpInt));
			var sendChatMessageAfterCallback = Method(() => Callbacks.Terraria.NetMessage.AfterSendChatMessageToClient(null, ref tmpColor, ref tmpInt));

			sendChatMessage.Wrap(sendChatMessageBeforeCallback, 
				sendChatMessageAfterCallback,
				beginIsCancellable: true,
				noEndHandling: true,
				allowCallbackInstance: false);

		}
	}
}
