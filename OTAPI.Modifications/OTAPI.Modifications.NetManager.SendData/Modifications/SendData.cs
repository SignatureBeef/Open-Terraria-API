using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.NetManager
{
    public class SendData : ModificationBase
    {
        public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
        {
            "TerrariaServer, Version=1.4.1.1, Culture=neutral, PublicKeyToken=null"
        };
        public override string Description => "Hooking NetManager.SendData...";

        public override void Run()
        {
            Terraria.Net.NetPacket packet = default;

            var vanilla = this.SourceDefinition.Type("Terraria.Net.NetManager").Method("SendData");
            var callback = this.Method(() => OTAPI.Callbacks.Terraria.NetManager.SendData(
                default, default, ref packet
            ));

            vanilla.Wrap
            (
                beginCallback: callback,
                endCallback: null,
                beginIsCancellable: true,
                noEndHandling: false,
                allowCallbackInstance: true
            );
        }
    }

    //public static class NetManager_Debug
    //{
    //	public static void SendData(global::Terraria.Net.Sockets.ISocket socket, global::Terraria.Net.NetPacket packet)
    //	{
    //		try
    //		{
    //			socket.AsyncSend(packet.Buffer.Data, 0, packet.Length, new global::Terraria.Net.Sockets.SocketSendCallback((object state) =>
    //			{
    //				((global::Terraria.Net.NetPacket)state).Recycle();
    //			}), packet);
    //		}
    //		catch (System.Exception ex)
    //		{
    //			System.Console.WriteLine("    Exception normal: Tried to send data to a client after losing connection" + ex);
    //		}
    //	}
    //}
}