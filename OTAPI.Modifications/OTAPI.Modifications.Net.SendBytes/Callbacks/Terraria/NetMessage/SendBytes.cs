namespace OTAPI.Core.Callbacks.Terraria
{
	internal static partial class NetMessage
	{
		/// <summary>
		/// This method replaces most calls to AsyncSend in the SendData method
		/// </summary>
		internal static void SendBytes
		(
			int remoteClient,
			byte[] data,
			int offset,
			int size,
			global::Terraria.Net.Sockets.SocketSendCallback callback,
			object state
		)
		{
			var result = Hooks.Net.SendBytes?.Invoke(ref remoteClient, ref data, ref offset, ref size, ref callback, ref state);
			if (result.HasValue && result.Value == HookResult.Cancel)
				return;

			global::Terraria.Netplay.Clients[remoteClient].Socket.AsyncSend(data, offset, size, callback, state);
		}
	}
}
