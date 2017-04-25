using OTAPI.Modifications.NetworkText;

namespace OTAPI
{
	public static partial class Hooks
	{
		public static class SendChatMessageToClient
		{
			public static BeforeChatMessageHandler BeforeSendChatMessageToClient;
			public static AfterChatMessageHandler AfterSendChatMessageToClient;
		}
	}
}
