namespace OTAPI.Callbacks.Terraria
{
    internal static partial class Netplay
    {
        internal static bool StartBroadCasting() => Hooks.Net.StartBroadCasting == null || Hooks.Net.StartBroadCasting() == HookResult.Continue;

        internal static bool StopBroadCasting() => Hooks.Net.StopBroadCasting == null || Hooks.Net.StopBroadCasting() == HookResult.Continue;
    }
}
