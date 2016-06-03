namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class NetMessage
    {
        public static bool SendData
        (
            ref int msgType,
            ref int remoteClient,
            ref int ignoreClient,
            ref string text,
            ref int number,
            ref float number2,
            ref float number3,
            ref float number4,
            ref int number5,
            ref int number6,
            ref int number7
        )
        {
            //Since we currently wrap the method we need to run these checks
            //as vanilla would have done this.
            if (global::Terraria.Main.netMode == (int)NetMode.SinglePlayer)
                return false;

            var bufferIndex = 256;
            if (global::Terraria.Main.netMode == (int)NetMode.Server && remoteClient >= 0)
                bufferIndex = remoteClient;

            if (Hooks.Net.SendData != null)
                return Hooks.Net.SendData
                (
                    ref bufferIndex,
                    ref msgType,
                    ref remoteClient,
                    ref ignoreClient,
                    ref text,
                    ref number,
                    ref number2,
                    ref number3,
                    ref number4,
                    ref number5,
                    ref number6,
                    ref number7
                ) == HookResult.Continue;
            return true;
        }
    }
}
