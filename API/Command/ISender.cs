using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace OTA.Command
{
    /// <summary>
    /// The bare implementation of a sender
    /// </summary>
    public interface ISender
    {
        bool Op { get; set; }

        string SenderName { get; }

        void SendMessage(string message, int sender = 255, byte R = 255, byte G = 255, byte B = 255);

        Dictionary<string, CommandInfo> GetAvailableCommands();
    }

    /// <summary>
    /// ISender extensions
    /// </summary>
    public static class ISenderExtensions
    {
        /// <summary>
        /// Sends a message
        /// </summary>
        /// <param name="recpt">Recpt.</param>
        /// <param name="message">Message.</param>
        public static void Message(this ISender recpt, string message)
        {
            recpt.SendMessage(message);
        }

        /// <summary>
        /// Sends a formatted message
        /// </summary>
        /// <param name="recpt">Recpt.</param>
        /// <param name="message">Message.</param>
        /// <param name="args">Arguments.</param>
        public static void Message(this ISender recpt, string message, params object[] args)
        {
            recpt.SendMessage(String.Format(message, args));
        }

        /// <summary>
        /// Sends a formatted message with colour if the sender supports it
        /// </summary>
        /// <param name="recpt">Recpt.</param>
        /// <param name="message">Message.</param>
        /// <param name="colour">Colour.</param>
        /// <param name="args">Arguments.</param>
        public static void Message(this ISender recpt, string message, Color colour, params object[] args)
        {
            recpt.SendMessage(String.Format(message, args), 255, colour.R, colour.G, colour.B);
        }

        public static void Message(this ISender recpt, int sender, string message)
        {
            recpt.SendMessage(message, sender);
        }

        public static void Message(this ISender recpt, int sender, Color color, string message)
        {
            recpt.SendMessage(message, sender, color.R, color.G, color.B);
        }

        public static void Message(this ISender recpt, int sender, string fmt, params object[] args)
        {
            recpt.SendMessage(String.Format(fmt, args), sender);
        }

        public static void Message(this ISender recpt, int sender, Color color, string fmt, params object[] args)
        {
            recpt.SendMessage(String.Format(fmt, args), sender, color.R, color.G, color.B);
        }

        /// <summary>
        /// Determines if is sender the specified sender type.
        /// </summary>
        /// <returns><c>true</c> if is sender the specified sender type; otherwise, <c>false</c>.</returns>
        /// <param name="sender">Sender.</param>
        /// <param name="type">Type.</param>
        public static bool IsSender(this ISender sender, SenderType type)
        {
            switch (type)
            {
                case SenderType.CONSOLE:
                    return sender is ConsoleSender;// && !(sender is RemoteConsole.RConSender);
#if Full_API
                case SenderType.PLAYER:
                    return sender is Terraria.Player;
#endif
            //case SenderType.RCON:
            //    return sender is RemoteConsole.RConSender;
                default:
                    throw new InvalidOperationException("No such sender.");
            }
        }

        public static SenderType GetSenderType(this ISender sender)
        {
            if (sender is ConsoleSender) // && !(sender is RemoteConsole.RConSender))
                return SenderType.CONSOLE;
            #if Full_API
            else if (sender is Terraria.Player)
                return SenderType.PLAYER;
            #endif
//            else if (sender is RemoteConsole.RConSender)
//                return SenderType.RCON;

            return SenderType.UNKNOWN;
        }
    }

    /// <summary>
    /// Default expected Sender Types.
    /// </summary>
    [Obsolete("This is not extensible")]
    public enum SenderType
    {
        CONSOLE,
        PLAYER,
        RCON,
        UNKNOWN
    }
}
