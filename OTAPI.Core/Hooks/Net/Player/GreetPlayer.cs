namespace OTAPI.Core
{
    public static partial class Hooks
    {
        public static partial class Net
        {
            public static partial class Player
            {
                #region Handlers
                public delegate HookResult PreGreetPlayerHandler(ref int playerId);
                public delegate void PostGreetPlayerHandler(int playerId);
                #endregion

                /// <summary>
                /// Occurs at the start of the greetPlayer method.
                /// Arg 1: playerid
                /// </summary>
                public static PreGreetPlayerHandler PreGreetPlayer;

                /// <summary>
                /// Occurs at the end of the greetPlayer method.
                /// Arg 1: playerid
                /// </summary>
                public static PostGreetPlayerHandler PostGreetPlayer;
            }
        }
    }
}
