namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class MessageBuffer
    {
        internal static int ReceiveData
        (
            global::Terraria.MessageBuffer buffer,
            ref byte packetId,
            ref int readOffset,
            ref int start,
            ref int length,
            ref int messageType
        )
        {
            if (Hooks.Net.ReceiveData != null)
            {
                if (Hooks.Net.ReceiveData(buffer, ref packetId, ref readOffset, ref start, ref length, ref messageType) == HookResult.Cancel)
                    return 0;
            }
            return (int)packetId;
        }
    }
}
