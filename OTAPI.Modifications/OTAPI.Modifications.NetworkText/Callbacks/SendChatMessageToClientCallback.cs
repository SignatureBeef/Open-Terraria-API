using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace OTAPI.Modifications.NetworkText.Callbacks
{
	internal static class SendChatMessageToClientCallback
	{
		/// <summary>
		/// Injected into the start of Terraria.NetMessage.BroadcastChatMessage.  Return HookResult.Cancelled to supress the broadcast message.
		/// </summary>
		/// <returns>True if the program is to run vanilla code, false otherwise.</returns>
		internal static bool BeforeSendChatMessageToClient(Terraria.Localization.NetworkText text, ref XnaColor color, ref int ignorePlayer)
		{
			var beforeChatMessageHook = Hooks.SendChatMessageToClient.BeforeSendChatMessageToClient;
			var nativeColor = Color.FromArgb(color.A, color.R, color.G, color.B);

			return (beforeChatMessageHook?.Invoke(text, ref nativeColor, ref ignorePlayer) ?? HookResult.Continue) == HookResult.Continue;
		}

		/// <summary>
		/// Injected into the start of Terraria.NetMessage.BroadcastChatMessage.  Return HookResult.Cancelled to supress the broadcast message.
		/// </summary>
		/// <returns>True if the program is to run vanilla code, false otherwise.</returns>
		internal static void AfterSendChatMessageToClient(Terraria.Localization.NetworkText text, ref XnaColor color, ref int ignorePlayer)
		{
			var beforeChatMessageHook = Hooks.SendChatMessageToClient.AfterSendChatMessageToClient;
			var nativeColor = Color.FromArgb(color.A, color.R, color.G, color.B);

			beforeChatMessageHook?.Invoke(text, ref nativeColor, ref ignorePlayer);
		}
	}
}
