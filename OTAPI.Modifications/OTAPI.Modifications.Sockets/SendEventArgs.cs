using System.Net.Sockets;
using Terraria.Net.Sockets;

namespace OTAPI.Sockets
{
	public class SendEventArgs : AsyncSocketEventArgs
	{
		public SocketSendCallback SendCallback { get; set; }

		protected override void OnCompleted(SocketAsyncEventArgs e)
		{
			base.OnCompleted(e);
			this.Socket.OnSendComplete(this);
		}
	}
}
