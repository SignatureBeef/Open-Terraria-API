using System.Net.Sockets;

namespace OTAPI.Sockets
{
	public class AsyncSocketEventArgs : SocketAsyncEventArgs
	{
		public AsyncSocket Socket { get; set; }
	}
}
