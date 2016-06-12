using System.Collections.Generic;
using System.Net.Sockets;

namespace OTAPI.Sockets
{
    public partial class PoolSocket : global::Terraria.Net.Sockets.ISocket
    {
        public class ReceiveEventArgs : PoolSocketEventArgs
        {
            protected override void OnCompleted(SocketAsyncEventArgs e)
            {
                base.OnCompleted(e);
                this.Socket.OnReceiveComplete(this);
            }
        }
        public class SendEventArgs : PoolSocketEventArgs
        {
            public List<Message> Confirmations { get; } = new List<Message>();

            public SendEventArgs()
            {
                this.UserToken = Confirmations;
            }

            protected override void OnCompleted(SocketAsyncEventArgs e)
            {
                base.OnCompleted(e);
                this.Socket.OnSendComplete(this);
            }
        }
    }
}
