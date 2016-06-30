namespace OTAPI.Core
{
    public static partial class Hooks
    {
        public static partial class Command
        {
            /// <summary>
            /// Occurs when the server is to start listening for commands.
            /// </summary>
            public static HookResultHandler StartCommandThread;
        }
    }
}
