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
            protected override void OnCompleted(SocketAsyncEventArgs e)
            {
                base.OnCompleted(e);
                this.Socket.OnSendComplete(this, this.UserToken as System.Collections.Generic.List<Message>);
            }
        }

        protected virtual void OnSendComplete(SendEventArgs arg, System.Collections.Generic.List<Message> messages)
        {
            System.Diagnostics.Debug.WriteLine($"Sent {arg.BytesTransferred} bytes");

            foreach (var msg in messages)
                msg.callback(msg.state);

            //Release socket
            arg.Socket = null;
            _sendPool.PushBack(arg);
        }
    }
}
