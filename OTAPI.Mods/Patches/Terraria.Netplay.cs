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

#if TerrariaServer_V1_4

using OTAPI;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Terraria
{
    class patch_Netplay : Terraria.Netplay
    {
        /** Begin Fix - Thread abort */
        public static bool BroadcastThreadActive = false;
        public extern static void orig_BroadcastThread();
        public static void BroadcastThread()
        {
            BroadcastThreadActive = true;
            orig_BroadcastThread();
        }

        public extern static void orig_StopBroadCasting();
        public static void StopBroadCasting()
        {
            BroadcastThreadActive = false;
            Terraria.Netplay.broadcastThread.Join();
            Terraria.Netplay.broadcastThread = null;

            orig_StopBroadCasting();
        }
        /** End Fix - Thread abort */
    }
}
#endif
