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
    public static class NetMessage
    {
        public static void SendBytes(Terraria.Net.Sockets.ISocket socket, byte[] data, int offset, int size, global::Terraria.Net.Sockets.SocketSendCallback callback, object state, int remoteClient)
        {
            if (Hooks.NetMessage.SendBytes?.Invoke(ref socket, ref remoteClient, ref data, ref offset, ref size, ref callback, ref state) != HookResult.Cancel)
            {
                socket.AsyncSend(data, offset, size, callback, state);
            }
        }
    }
}
