using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Threading;
using OTA.Plugin;
using System.Net.Sockets;
using System.Net;
using OTA.Sockets;
using OTA.Logging;

#if Full_API
using Terraria.Net.Sockets;
using Terraria.Net;
#endif

namespace OTA.Callbacks
{
    /// <summary>
    /// Callbacks from Terraria.Netplay
    /// </summary>
    public static class NetplayCallback
    {
        /// <summary>
        /// When vanilla requests to start the server, this method is called.
        /// </summary>
        /// <param name="state">State.</param>
        public static void StartServer(object state)
        {
#if Full_API && SERVER
            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.ServerStateChange()
            {
                ServerChangeState = (Globals.CurrentState = ServerState.Starting)
            };
            HookPoints.ServerStateChange.Invoke(ref ctx, ref args);

            if (ctx.Result != HookResult.IGNORE)
            {
                ProgramLog.Console.Print("Starting server...");
                ThreadPool.QueueUserWorkItem(new WaitCallback(Terraria.Netplay.ServerLoop), 1);
                ProgramLog.Console.Print("Ok");
            }
#endif
        }

        /// <summary>
        /// Called upon a connection of a new slot
        /// </summary>
        /// <param name="slot">Slot.</param>
        public static void OnNewConnection(int slot)
        {
            #if Full_API
            if (Terraria.Netplay.Clients[slot].Socket is ClientConnection)
            {
                ((ClientConnection)Terraria.Netplay.Clients[slot].Socket).Set(slot);
            }
            #endif
        }

        /// <summary>
        /// The callback from Terraria.Netplay.Initialize
        /// </summary>
        public static bool Initialise()
        {
            #if Full_API && SERVER
            if (Terraria.Main.dedServ)
            {
                var ctx = new HookContext()
                {
                    Sender = HookContext.ConsoleSender
                };
                var args = new HookArgs.ServerStateChange()
                {
                    ServerChangeState = ServerState.Starting
                };
                HookPoints.ServerStateChange.Invoke(ref ctx, ref args);

                return ctx.Result != HookResult.IGNORE;
            }
            #endif

            return true; //Allow continue
        }

        public static void OnServerFull(ISocket client)
        {
            #if Full_API && SERVER
            var conn = client as ClientConnection;
            conn.Kick("Server is full");
            #endif
        }

        #if SERVER
        public static bool OnAddBanBegin(int player)
        {
            var ctx = new HookContext();
            var args = new HookArgs.AddBan()
            {
                State = MethodState.Begin,
                Slot = player
            };

            HookPoints.AddBan.Invoke(ref ctx, ref args);

            return ctx.Result == HookResult.DEFAULT;
        }

        public static void OnAddBanEnd(int player)
        {
            var ctx = new HookContext();
            var args = new HookArgs.AddBan()
            {
                State = MethodState.End,
                Slot = player
            };

            HookPoints.AddBan.Invoke(ref ctx, ref args);
        }
        #endif
    }
}