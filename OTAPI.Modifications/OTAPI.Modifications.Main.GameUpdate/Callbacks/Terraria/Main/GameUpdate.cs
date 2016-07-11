using Microsoft.Xna.Framework;

namespace OTAPI.Core.Callbacks.Terraria
{
	internal static partial class Main
	{
		/// <summary>
		/// This method is injected to the beginning of the terraria Update method.
		/// </summary>
		/// <param name="gameTime"></param>
		internal static void UpdateBegin(global::Terraria.Main game, ref GameTime gameTime) => Hooks.Game.PreUpdate?.Invoke(ref gameTime);

		/// <summary>
		/// This method is injected into the end of the terraria Update method.
		/// </summary>
		/// <param name="gameTime"></param>
		internal static void UpdateEnd(global::Terraria.Main game, ref GameTime gameTime) => Hooks.Game.PostUpdate?.Invoke(ref gameTime);
	}
}
