namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Main
    {
        internal static bool OnChatTextSend()
        {
            if (Hooks.Input.ChatSend?.Invoke() == HookResult.Cancel)
                return false;

            return true;
        }
    }
}