using System;
using Terraria;
using OTA.Extensions;

namespace OTA.Mod.Net
{
    public static class RemoteClientExtensions
    {
        public static bool HasNpcTexture(this RemoteClient client, int typeId)
        {
            return client.GetData("NpcTexture_" + typeId, false);
        }

        public static void SetHasNpcTexture(this RemoteClient client, int typeId, bool value)
        {
            client.SetData("NpcTexture_" + typeId, value);
        }
    }
}

