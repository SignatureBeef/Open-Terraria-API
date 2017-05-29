using Microsoft.Xna.Framework;

namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class Net
		{
			public delegate HookResult BeforeChatMessageHandler(global::Terraria.Localization.NetworkText text, ref Color color, ref int ignorePlayer);
			public delegate void AfterChatMessageHandler(global::Terraria.Localization.NetworkText text, ref Color color, ref int ignorePlayer);
		}
	}
}