using Terraria.Net.Sockets;

namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Netplay
	{
		public static bool OnConnectionAccepted(ISocket client)
		{
			var result = Hooks.Net.Socket.Accepted?.Invoke(client);
			if (result != null && result.Value == HookResult.Cancel)
				return false;

			return true;
		}
	}
}
