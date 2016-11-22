using Terraria.Net.Sockets;

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Net
        {
            public static partial class Socket
            {
                #region Handlers
                public delegate HookResult AcceptedHandler(ISocket client);
                #endregion
				
                public static AcceptedHandler Accepted;
            }
        }
    }
}
