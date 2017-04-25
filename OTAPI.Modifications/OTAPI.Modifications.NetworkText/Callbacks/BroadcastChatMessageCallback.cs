using Microsoft.Xna.Framework;

namespace OTAPI.Modifications.NetworkText.Callbacks
{
	internal static class BroadcastChatMessageCallback
	{
		/// <summary>
		/// Injected into the start of Terraria.NetMessage.BroadcastChatMessage.  Return HookResult.Cancelled to supress the broadcast message.
		/// </summary>
		/// <returns>True if the program is to run vanilla code, false otherwise.</returns>
		internal static bool BeforeBroadcastChatMessage(Terraria.Localization.NetworkText text, ref Color color, ref int ignorePlayer)
		{
			var beforeChatMessageHook = Hooks.BroadcastChatMessage.BeforeBroadcastChatMessage;

			return (beforeChatMessageHook?.Invoke(text, ref color, ref ignorePlayer) ?? HookResult.Continue) == HookResult.Continue;
		}

		/// <summary>
		/// Injected into the start of Terraria.NetMessage.BroadcastChatMessage.  Return HookResult.Cancelled to supress the broadcast message.
		/// </summary>
		/// <returns>True if the program is to run vanilla code, false otherwise.</returns>
		internal static void AfterBroadcastChatMessage(Terraria.Localization.NetworkText text, ref Color color, ref int ignorePlayer)
		{
			Hooks.BroadcastChatMessage.AfterBroadcastChatMessage?.Invoke(text, ref color, ref ignorePlayer);
		}
	}
}
