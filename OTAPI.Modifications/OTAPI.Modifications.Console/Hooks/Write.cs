namespace OTAPI.Core
{
    public static partial class Hooks
    {
        public static partial class Console
        {
            #region Handlers
            public delegate HookResult WriteHandler(string message);
            #endregion

            /// <summary>
            /// Occurs each time vanilla calls Console.Write
            /// </summary>
            public static WriteHandler Write;
        }
    }
}
