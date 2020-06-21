/*
Copyright (C) 2020 DeathCradle

This file is part of Open Terraria API v3 (OTAPI)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/

namespace OTAPI.Callbacks
{
    public static class MessageBuffer
    {
        public static bool GetData(global::Terraria.MessageBuffer instance, ref byte packetId, ref int readOffset, ref int start, ref int length, ref int messageType, int maxPackets)
            => Hooks.MessageBuffer.GetData?.Invoke(instance, ref packetId, ref readOffset, ref start, ref length, ref messageType, ref maxPackets) != HookResult.Cancel && packetId < maxPackets;

        /// <summary>
        /// Called when Terraria receives a ClientUUID(#68) packet from a connection
        /// </summary>
        public static void ReadClientUUID(Terraria.MessageBuffer instance, System.IO.BinaryReader reader, int start, int length, ref int messageType)
        {
            if (Hooks.MessageBuffer.ClientUUIDReceived?.Invoke(HookEvent.Before, instance, reader, start, length, messageType) != HookResult.Cancel)
            {
                var clientUUID = reader.ReadString();

                ((Terraria.patch_RemoteClient)Terraria.Netplay.Clients[instance.whoAmI]).ClientUUID = clientUUID;

                Hooks.MessageBuffer.ClientUUIDReceived?.Invoke(HookEvent.After, instance, reader, start, length, messageType);
            }
        }
    }
}
