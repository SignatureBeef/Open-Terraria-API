namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Console
    {
        internal static void WriteLine(string message)
        {
            if (Hooks.Console.WriteLine?.Invoke(message) == HookResult.Cancel)
                return;

            System.Console.WriteLine(message);
        }
    }
}
