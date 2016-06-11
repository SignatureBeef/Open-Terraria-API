namespace OTAPI.Sockets
{
    public partial class PoolSocket : global::Terraria.Net.Sockets.ISocket
    {
        private static ArgsPool<ReceiveEventArgs> _receivePool = new ArgsPool<ReceiveEventArgs>();
        private static ArgsPool<SendEventArgs> _sendPool = new ArgsPool<SendEventArgs>();
    }
}
