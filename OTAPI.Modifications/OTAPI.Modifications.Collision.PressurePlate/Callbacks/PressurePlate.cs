namespace OTAPI.Callbacks.Terraria
{
	internal partial class Collision
	{
		internal static void PressurePlate(int x, int y, IEntity entity)
		{
			if (Hooks.Collision.PressurePlate?.Invoke(ref x, ref y, ref entity) == HookResult.Cancel)
			{
				return;
			}

			// In the patcher the below code is removed so we must action the logic ourselves.
			// Broadcast to everyone.
			global::Terraria.Wiring.HitSwitch(x, y);
			global::Terraria.NetMessage.TrySendData(59, number: x, number2: y);
		}

		internal static void HitSwitch(int x, int y, IEntity entity)
		{
			if (Hooks.Collision.PressurePlate?.Invoke(ref x, ref y, ref entity) == HookResult.Cancel)
			{
				return;
			}

			var player = entity as global::Terraria.Player;

			// In the patcher the below code is removed so we must action the logic ourselves.
			// Broadcast to everyone except the player who triggered it.
			global::Terraria.Wiring.HitSwitch(x, y);
			global::Terraria.NetMessage.TrySendData(59, number: x, number2: y, ignoreClient: player.whoAmI);
		}
	}
}
