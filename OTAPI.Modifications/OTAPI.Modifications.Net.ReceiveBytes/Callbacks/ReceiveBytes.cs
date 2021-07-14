namespace OTAPI.Callbacks.Terraria
{
    internal static partial class NetMessage
    {
        internal static bool ReceiveBytes(ref byte[] bytes, ref int streamLength, ref int bufferIndex)
        {
            var result = Hooks.Net.ReceiveBytes?.Invoke(ref bytes, ref streamLength, ref bufferIndex);
            if (result.HasValue && result.Value == HookResult.Cancel)
                return false;

            return true;
        }
    }
}
