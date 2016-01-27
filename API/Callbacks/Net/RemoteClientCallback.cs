#if SERVER
using System;
using OTA.Plugin;
using OTA.Extensions;

namespace OTA.Callbacks
{
    public static class RemoteClientCallback
    {
        public static void OnReset(Terraria.RemoteClient client)
        {
            var ctx = HookContext.Empty;
            var args = new HookArgs.RemoteClientReset()
            {
                Client = client
            };

            HookPoints.RemoteClientReset.Invoke(ref ctx, ref args);

            client.ClearData();
        }
    }
}
#endif