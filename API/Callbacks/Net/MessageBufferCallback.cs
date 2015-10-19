using Microsoft.Xna.Framework;
using System;
using OTA.ID;
using OTA.Plugin;
using OTA.Logging;
using System.Collections.Concurrent;

#if Full_API
using Terraria;
#endif
namespace OTA.Callbacks
{
    /// <summary>
    /// The callbacks from the terrarian tcp clients' stream reader
    /// </summary>
    public static class MessageBufferCallback
    {
        //When adding a packet ensure the section in the case ends with returning. These packets are safe to filter
        //Ones without a return statement will mean the code at the end of GetData doesnt get called;

        /// <summary>
        /// The call from within Terraria.MessageBuffer.GetData
        /// </summary>
        /// <returns>0 when consumed, the original packet id if vanilla code is to process as normal</returns>
        /// <param name="bufferId">Buffer identifier.</param>
        /// <param name="packetId">Packet identifier.</param>
        /// <param name="start">Start of the net message.</param>
        /// <param name="length">Length of the net message.</param>
        public static byte ProcessPacket(int bufferId, byte packetId, int start, int length)
        {
            #if Full_API

            var ctx = new HookContext()
            {
                Connection = Netplay.Clients[bufferId].Socket,
                Player = Main.player[bufferId],
                Sender = Main.player[bufferId]
            };
            var args = new HookArgs.ReceiveNetMessage()
            {
                BufferId = bufferId,
                PacketId = packetId,
                Start = start,
                Length = length
            };
            HookPoints.ReceiveNetMessage.Invoke(ref ctx, ref args);

            if (ctx.Result == HookResult.IGNORE)
            {
                return 0;
            }
            else if (ctx.Result == HookResult.RECTIFY)
            {
                return (byte)ctx.ResultParam;
            }

            return packetId;
#else
            return 0;
#endif
        }

        /// <summary>
        /// The callback check to see if a client sent a wrong message at the wrong state
        /// </summary>
        /// <param name="bufferId"></param>
        /// <param name="packetId"></param>
        /// <returns></returns>
        public static bool CheckForInvalidState(int bufferId, byte packetId)
        {
            var ctx = new HookContext();
            var args = new HookArgs.CheckBufferState()
            {
                BufferId = bufferId,
                PacketId = packetId
            };

            HookPoints.CheckBufferState.Invoke(ref ctx, ref args);

            if (ctx.Result == HookResult.RECTIFY && ctx.ResultParam is bool)
            {
                return (bool)ctx.ResultParam;
            }

            return Main.netMode == 2 && Netplay.Clients[bufferId].State < 10
            && packetId > 12 && packetId != 93 && packetId != 16 && packetId != 42
            && packetId != 50 && packetId != 38 && packetId != 68;
        }
    }
}
