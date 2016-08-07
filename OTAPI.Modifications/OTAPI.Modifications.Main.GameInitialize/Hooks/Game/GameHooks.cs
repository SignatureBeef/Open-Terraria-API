namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Game
        {
            /// <summary>
            /// Occurs at the first point of call when the Initialze method is ran.
            /// </summary>
            public static HookHandler PreInitialize;

            /// <summary>
            /// Occurs at the end of the games initialize method.
            /// </summary>
            public static HookHandler PostInitialize;
        }
    }
}
