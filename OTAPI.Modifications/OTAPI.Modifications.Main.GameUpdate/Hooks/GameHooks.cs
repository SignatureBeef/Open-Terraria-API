namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Game
        {
            #region Handlers
            public delegate void UpdateHandler(ref Microsoft.Xna.Framework.GameTime gameTime);
            #endregion

            /// <summary>
            /// Occurs at the first point of call when the game is running the update loop.
            /// </summary>
            public static UpdateHandler PreUpdate;

            /// <summary>
            /// Occurs at the end of the games update loop.
            /// </summary>
            public static UpdateHandler PostUpdate;
        }
    }
}
