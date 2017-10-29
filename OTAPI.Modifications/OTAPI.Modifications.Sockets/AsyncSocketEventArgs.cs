using System.Net.Sockets;

namespace OTAPI.Sockets
{
	public class AsyncSocketEventArgs : SocketAsyncEventArgs
	{
		public volatile AsyncClientSocket conn;

		//public long Id { get; internal set; }

		protected override void OnCompleted(SocketAsyncEventArgs e)
		{
		}
	}
}
