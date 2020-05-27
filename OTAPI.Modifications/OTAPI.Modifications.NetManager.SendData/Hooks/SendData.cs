using Terraria.Net;
using Terraria.Net.Sockets;

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Net
        {
            #region Handlers
            public delegate HookResult SendNetDataHandler
            (
                NetManager manager,
                ISocket socket,
                ref NetPacket packet
            );
            #endregion

            public static SendNetDataHandler SendNetData;
        }
    }
}
