using System.Net.Sockets;
using Terraria.Net.Sockets;

namespace OTAPI.Sockets
{
	public class ReceiveEventArgs : AsyncSocketEventArgs
	{
		public SocketReceiveCallback ReceiveCallback { get; set; }

		protected override void OnCompleted(SocketAsyncEventArgs e)
		{
			base.OnCompleted(e);
			this.Socket.OnReceiveComplete(this);
		}
	}
}
