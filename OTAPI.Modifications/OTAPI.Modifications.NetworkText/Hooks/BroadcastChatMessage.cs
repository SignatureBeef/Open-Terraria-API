using OTAPI.Modifications.NetworkText;

namespace OTAPI
{
	public static partial class Hooks
	{
		public static class BroadcastChatMessage
		{
			public static BeforeChatMessageHandler BeforeBroadcastChatMessage;
			public static AfterChatMessageHandler AfterBroadcastChatMessage;
		}
	}
}
