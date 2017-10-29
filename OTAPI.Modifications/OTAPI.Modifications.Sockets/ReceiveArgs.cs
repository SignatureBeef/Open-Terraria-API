using System.Net.Sockets;

namespace OTAPI.Sockets
{
	public class ReceiveArgs : AsyncSocketEventArgs
	{
		public volatile Socket origin;

		protected override void OnCompleted(SocketAsyncEventArgs e)
		{
			conn?.ReceiveCompleted(this);
		}
	}
}
