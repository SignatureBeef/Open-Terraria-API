using System;

namespace OTAPI.Core.Hooks.Terraria
{
    public static class Main
    {
        /// <summary>
        /// Occurs when the server is to start listening for commands.
        /// </summary>
        public static Func<Boolean> OnStartCommandThread;

        /// <summary>
        /// Occurs at the first point of call when the game is running the update loop.
        /// </summary>
        public static Action OnGamePreUpdate;

        /// <summary>
        /// Occurs at the end of the games update loop.
        /// </summary>
        public static Action OnGamePostUpdate;

        /// <summary>
        /// This method is injected into the start of the startDedInput method.
        /// The return value will dictate if normal vanilla code should continue to run.
        /// </summary>
        /// <returns>True to continue on to vanilla, otherwise false</returns>
        internal static bool startDedInput()
        {
            if (OnStartCommandThread != null)
                return OnStartCommandThread.Invoke();
            return true;
        }

        /// <summary>
        /// This method is injected to the beginning of the terraria Update method.
        /// </summary>
        /// <param name="gameTime"></param>
        internal static void UpdateBegin(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (OnGamePreUpdate != null)
                OnGamePreUpdate();
        }

        /// <summary>
        /// This method is injected into the end of the terraria Update method.
        /// </summary>
        /// <param name="gameTime"></param>
        internal static void UpdateEnd(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (OnGamePostUpdate != null)
                OnGamePostUpdate();
        }
    }
}
