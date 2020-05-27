using Terraria.Net;
using Terraria.Net.Sockets;

namespace OTAPI.Callbacks.Terraria
{
    internal static partial class NetManager
    {
        internal static bool SendData
        (
            global::Terraria.Net.NetManager manager,
            ISocket socket,
            ref NetPacket packet
        )
        {
            var res = Hooks.Net.SendNetData?.Invoke
            (
                manager,
                socket,
                ref packet
            );
            if (res.HasValue) return res.Value == HookResult.Continue;
            return true;
        }
    }
}
