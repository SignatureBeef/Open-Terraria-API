namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Main
    {
        /// <summary>
        /// This method is injected to the beginning of the terraria Update method.
        /// </summary>
        /// <param name="gameTime"></param>
        internal static void UpdateBegin(ref Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (Hooks.Game.PreUpdate != null)
                Hooks.Game.PreUpdate(ref gameTime);
        }

        /// <summary>
        /// This method is injected into the end of the terraria Update method.
        /// </summary>
        /// <param name="gameTime"></param>
        internal static void UpdateEnd(ref Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (Hooks.Game.PostUpdate != null)
                Hooks.Game.PostUpdate(ref gameTime);
        }
    }
}
