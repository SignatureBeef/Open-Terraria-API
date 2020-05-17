using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Net
{
    public class SendData : ModificationBase
    {
        public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
        {
            "TerrariaServer, Version=1.4.0.0, Culture=neutral, PublicKeyToken=null"
        };
        public override string Description => "Hooking NetMessage.SendData...";

        public override void Run()
        {
            int tmpI = 0;
            float tmpF = 0;
            Terraria.Localization.NetworkText tmpS = null;

            var vanilla = this.Method(() => Terraria.NetMessage.SendData(0, -1, -1, Terraria.Localization.NetworkText.Empty, 0, 0, 0, 0, 0, 0, 0));
            var callback = this.Method(() => OTAPI.Callbacks.Terraria.NetMessage.SendData(
                ref tmpI, ref tmpI, ref tmpI, ref tmpS, ref tmpI, ref tmpF, ref tmpF, ref tmpF, ref tmpI, ref tmpI, ref tmpI
            ));

            //Few stack issues arose trying to inject a callback before for lock, so i'll resort to 
            //wrapping the method;

            vanilla.Wrap
            (
                beginCallback: callback,
                endCallback: null,
                beginIsCancellable: true,
                noEndHandling: false,
                allowCallbackInstance: false
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