namespace OTAPI.Callbacks.Terraria
{
    internal static partial class MessageBuffer
    {
        internal static int ReceiveData
        (
            global::Terraria.MessageBuffer buffer,
            ref byte packetId,
            ref int readOffset,
            ref int start,
            ref int length
        )
        {
            if (Hooks.Net.ReceiveData?.Invoke(buffer, ref packetId, ref readOffset, ref start, ref length) == HookResult.Cancel)
                return packetId = byte.MaxValue;
            return (int)packetId;
        }
    }
}
