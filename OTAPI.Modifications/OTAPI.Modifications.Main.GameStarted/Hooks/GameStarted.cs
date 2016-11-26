namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Game
        {
            /// <summary>
            /// Occurs right after server initialisation and before the server loop starts
            /// </summary>
            public static HookHandler Started;
        }
    }
}
