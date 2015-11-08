using System;
using OTA.Command;
using OTA.Extensions;

using Terraria;

namespace OTA.Plugin
{
    public struct HookContext
    {
        /// <summary>
        /// A Reusable empty context for when the context does not matter.
        /// </summary>
        public static readonly HookContext Empty = new HookContext();

        /// <summary>
        /// Gets or sets the sender whom triggered the event.
        /// </summary>
        /// <value>The sender.</value>
        public ISender Sender { get; set; }

        #if Full_API
        /// <summary>
        /// Gets or sets the connection that triggered the event
        /// </summary>
        /// <value>The connection.</value>
        public Terraria.Net.Sockets.ISocket Connection { get; set; }

        /// <summary>
        /// Gets the <see cref="Terraria.RemoteClient"/> instance for the Player associated.
        /// </summary>
        /// <remarks>If the Player property is null on this instance then this value will also be null.</remarks>
        /// <value>The client.</value>
        public RemoteClient Client
        {
            get
            {
                if (Player != null)
                {
                    var slot = Player.whoAmI;
                    return Terraria.Netplay.Clients[slot];
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the player associated with the event (if applicable)
        /// </summary>
        /// <value>The player.</value>
        public Player Player { get; set; }

        #else
        public OTA.Sockets.ISocket Connection { get; set; }
        #endif

        /// <summary>
        /// The result parameter that the hook may use after completely called.
        /// </summary>
        /// <value>The result parameter.</value>
        public object ResultParam { get; private set; }

        /// <summary>
        /// The action the hook must do when after the hook is completed.
        /// </summary>
        /// <value>The result.</value>
        public HookResult Result { get; private set; }

        /// <summary>
        /// This will immediately stop further hook execution, and will return to the hook caller.
        /// </summary>
        /// <value><c>true</c> if conclude; otherwise, <c>false</c>.</value>
        public bool Conclude { get; set; }

        public static readonly ConsoleSender ConsoleSender = new ConsoleSender();

        /// <summary>
        /// This will check if the current Result is set to KICK. 
        /// If so it will perform the kick to remove the player from the server, and will use
        /// the ResultParam value as the disconnect readon.
        /// </summary>
        /// <returns><c>true</c>, if the connection/player was kicked, <c>false</c> otherwise.</returns>
        public bool CheckForKick()
        {
            #if Full_API
            if (Connection != null)
            {
                if (Result == HookResult.KICK)
                {
                    var reason = ResultParam as string;
                    if (Client != null)
                    {
                        Client.Kick(reason ?? "Connection closed by plugin.");
                    }
                    else
                    {
                        Connection.Close();
                    }
                    return true;
                }
                //                else if (Connection.DisconnectInProgress())
                else if (Client != null && Client.PendingTermination)
                {
                    return true;
                }
            }
            #endif

            return false;
        }

        /// <summary>
        /// Sets the result for the hook and by default will return to the hook initiator for appropriate processing.
        /// </summary>
        /// <param name="result">Result for the intiator to action</param>
        /// <param name="conclude">If set to <c>true</c> conclude (no further plugins will receive the hook).</param>
        /// <param name="resultParam">Result parameter for the initiator (if applicable).</param>
        public void SetResult(HookResult result, bool conclude = true, object resultParam = null)
        {
            Result = result;
            ResultParam = resultParam;
            Conclude = conclude;
        }

        /// <summary>
        /// Marks that the initiator should kick the player/connection after processing (if applicable).
        /// </summary>
        /// <param name="reason">Reason to be sent to the player.</param>
        /// <param name="conclude">If set to <c>true</c> conclude (no further plugins will receive the hook).</param>
        public void SetKick(string reason, bool conclude = true)
        {
            Result = HookResult.KICK;
            ResultParam = reason;
            Conclude = conclude;
        }

        /// <summary>
        /// Set's the result parameter for the initiater to use.
        /// </summary>
        /// <param name="result">Result.</param>
        public void SetParam(object result)
        {
            ResultParam = result;
        }
    }
}

