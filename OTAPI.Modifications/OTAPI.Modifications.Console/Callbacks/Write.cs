namespace OTAPI.Callbacks.Terraria
{
    internal partial class Console
    {
        internal static void Write(string message)
        {
            if (Hooks.Console.Write?.Invoke(message) == HookResult.Cancel)
                return;

            System.Console.Write(message);
        }

        internal static void Write(string format, object arg0, object arg1)
        {
            if (Hooks.Console.Write?.Invoke(format) == HookResult.Cancel)
                return;

            System.Console.Write(format, arg0, arg1);
        }
    }
}
