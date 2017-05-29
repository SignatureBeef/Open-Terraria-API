using Microsoft.Xna.Framework;

namespace OTAPI.Callbacks.Terraria
{
	internal static partial class NetMessage
	{
		/// <summary>
		/// Injected into the start of Terraria.NetMessage.BroadcastChatMessage.  Return HookResult.Cancelled to supress the broadcast message.
		/// </summary>
		/// <returns>True if the program is to run vanilla code, false otherwise.</returns>
		internal static bool BeforeBroadcastChatMessage(global::Terraria.Localization.NetworkText text, ref Color color, ref int ignorePlayer)
		{
			var beforeChatMessageHook = Hooks.Net.BeforeBroadcastChatMessage;

			return (beforeChatMessageHook?.Invoke(text, ref color, ref ignorePlayer) ?? HookResult.Continue) == HookResult.Continue;
		}

		/// <summary>
		/// Injected into the start of Terraria.NetMessage.BroadcastChatMessage.  Return HookResult.Cancelled to supress the broadcast message.
		/// </summary>
		/// <returns>True if the program is to run vanilla code, false otherwise.</returns>
		internal static void AfterBroadcastChatMessage(global::Terraria.Localization.NetworkText text, ref Color color, ref int ignorePlayer)
		{
			Hooks.Net.AfterBroadcastChatMessage?.Invoke(text, ref color, ref ignorePlayer);
		}
	}
}
