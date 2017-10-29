using System.Net.Sockets;

namespace OTAPI.Sockets
{
	public delegate void ConnectionDisconnectHandler(AsyncClientSocket connection);
	public delegate void SocketAccepted(Socket socket);
}
