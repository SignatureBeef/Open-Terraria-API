using System.IO;

namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class NetMessage
    {
        //public class PacketWriter : BinaryWriter
        //{
        //    private MemoryStream ms;

        //    public PacketWriter() : base()
        //    {
        //        this.OutStream = ms;
        //    }
        //}

        // Terraria.NetMessage
        public static void SendData(int msgType, int remoteClient = -1, int ignoreClient = -1, string text = "", int number = 0, float number2 = 0f, float number3 = 0f, float number4 = 0f, int number5 = 0, int number6 = 0, int number7 = 0)
        {

        }
    }
}
