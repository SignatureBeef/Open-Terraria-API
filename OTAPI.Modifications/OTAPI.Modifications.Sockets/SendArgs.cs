using System.Net.Sockets;
using Terraria.Net.Sockets;

namespace OTAPI.Sockets
{
	public class SendArgs : AsyncSocketEventArgs
	{
		public volatile SocketSendCallback callback;
		public volatile object state;

		protected override void OnCompleted(SocketAsyncEventArgs e)
		{
			conn?.SendCompleted(this);
		}
	}
}
