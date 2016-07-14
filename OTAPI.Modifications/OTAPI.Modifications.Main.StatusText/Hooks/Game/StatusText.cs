namespace OTAPI.Core
{
    public static partial class Hooks
    {
        public static partial class Game
        {
            #region Handlers
            public delegate HookResult StatusTextHandler(ref string text);
            #endregion

            /// <summary>
            /// Occurs when Terraria.Main.DedServ is checking if it needs to write console data
            /// </summary>
            public static HookResultHandler StatusTextUpdate;

            /// <summary>
            /// Occurs when the Terraria.Main.statusText variable is written to
            /// </summary>
            public static StatusTextHandler StatusTextWrite;
        }
    }
}
