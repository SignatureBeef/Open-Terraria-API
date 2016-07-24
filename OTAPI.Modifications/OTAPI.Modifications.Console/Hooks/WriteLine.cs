namespace OTAPI.Core
{
    public static partial class Hooks
    {
        public static partial class Console
        {
            #region Handlers
            public delegate HookResult WriteLineHandler(string message);
            #endregion

            /// <summary>
            /// Occurs each time vanilla calls Console.WriteLine
            /// </summary>
            public static WriteLineHandler WriteLine;
        }
    }
}
