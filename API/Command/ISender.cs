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
    }
}
