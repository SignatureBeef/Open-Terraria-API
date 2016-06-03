namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class MessageBuffer
    {
        public static int ReceiveData
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
                Hooks.Net.ReceiveData(buffer, ref packetId, ref readOffset, ref start, ref length, ref messageType);
            return (int)packetId;
        }
    }
}
