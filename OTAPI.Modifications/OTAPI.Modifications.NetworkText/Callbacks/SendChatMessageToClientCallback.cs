using Microsoft.Xna.Framework;

namespace OTAPI.Modifications.NetworkText.Callbacks
{
	internal static class SendChatMessageToClientCallback
	{
		/// <summary>
		/// Injected into the start of Terraria.NetMessage.BroadcastChatMessage.  Return HookResult.Cancelled to supress the broadcast message.
		/// </summary>
		/// <returns>True if the program is to run vanilla code, false otherwise.</returns>
		internal static bool BeforeSendChatMessageToClient(Terraria.Localization.NetworkText text, ref Color color, ref int ignorePlayer)
		{
			var beforeChatMessageHook = Hooks.SendChatMessageToClient.BeforeSendChatMessageToClient;

			return (beforeChatMessageHook?.Invoke(text, ref color, ref ignorePlayer) ?? HookResult.Continue) == HookResult.Continue;
		}

		/// <summary>
		/// Injected into the start of Terraria.NetMessage.BroadcastChatMessage.  Return HookResult.Cancelled to supress the broadcast message.
		/// </summary>
		/// <returns>True if the program is to run vanilla code, false otherwise.</returns>
		internal static void AfterSendChatMessageToClient(Terraria.Localization.NetworkText text, ref Color color, ref int ignorePlayer)
		{
			var beforeChatMessageHook = Hooks.SendChatMessageToClient.AfterSendChatMessageToClient;

			beforeChatMessageHook?.Invoke(text, ref color, ref ignorePlayer);
		}
	}
}
