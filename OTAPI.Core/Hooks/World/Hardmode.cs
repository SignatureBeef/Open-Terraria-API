namespace OTAPI.Core
{
    public static partial class Hooks
    {
        public static partial class World
        {
            #region Handlers
            public delegate HookResult PreHardmodeHandler();
            public delegate void PostHardmodeHandler();
            #endregion

            /// <summary>
            /// Occurs at the start of the StartHardmode() method.
            /// </summary>
            public static PreHardmodeHandler PreHardmode;

            /// <summary>
            /// Occurs when the StartHardmode() method ends
            /// </summary>
            public static PostHardmodeHandler PostHardmode;
        }
    }
}
