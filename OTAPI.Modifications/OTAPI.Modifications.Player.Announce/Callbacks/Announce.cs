namespace OTAPI.Callbacks.Terraria
{
	internal static partial class NetMessage
	{
		internal static bool Announce(int playerId)
		{
			var res = Hooks.Player.Announce?.Invoke(playerId);
			if (res.Value == HookResult.Cancel)
			{
				if (global::Terraria.Netplay.Clients[playerId].IsAnnouncementCompleted)
				{
					global::Terraria.Netplay.Clients[playerId].Name = "Anonymous";
				}
				return false;
			}

			return true;
		}
	}
}
