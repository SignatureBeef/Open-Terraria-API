using Microsoft.Xna.Framework;

namespace OTAPI.Callbacks.Terraria
{
	internal static partial class NetMessage
	{
		/// <summary>
		/// Injected into the start of Terraria.NetMessage.BroadcastChatMessage.  Return HookResult.Cancelled to supress the broadcast message.
		/// </summary>
		/// <returns>True if the program is to run vanilla code, false otherwise.</returns>
		internal static bool BeforeSendChatMessageToClient(global::Terraria.Localization.NetworkText text, ref Color color, ref int ignorePlayer)
		{
			var beforeChatMessageHook = Hooks.Net.BeforeSendChatMessageToClient;

			return (beforeChatMessageHook?.Invoke(text, ref color, ref ignorePlayer) ?? HookResult.Continue) == HookResult.Continue;
		}

		/// <summary>
		/// Injected into the start of Terraria.NetMessage.BroadcastChatMessage.  Return HookResult.Cancelled to supress the broadcast message.
		/// </summary>
		/// <returns>True if the program is to run vanilla code, false otherwise.</returns>
		internal static void AfterSendChatMessageToClient(global::Terraria.Localization.NetworkText text, ref Color color, ref int ignorePlayer)
		{
			var beforeChatMessageHook = Hooks.Net.AfterSendChatMessageToClient;

			beforeChatMessageHook?.Invoke(text, ref color, ref ignorePlayer);
		}
	}
}
