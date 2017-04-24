using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTAPI
{
	public static partial class Hooks
	{
		public static class BroadcastChatMessage
		{
			public delegate HookResult BeforeBroadcastChatMessageHandler(global::Terraria.Localization.NetworkText text, ref Color color, ref int ignorePlayer);
			public delegate void AfterBroadcastChatMessageHandler(global::Terraria.Localization.NetworkText text, ref Color color, ref int ignorePlayer);

			public static BeforeBroadcastChatMessageHandler BeforeBroadcastChatMessage;
			public static AfterBroadcastChatMessageHandler AfterBroadcastChatMessage;

		}
	}
}
