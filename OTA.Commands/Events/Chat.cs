using OTA.Plugin;
using Microsoft.Xna.Framework;

namespace OTA.Commands.Events
{
    public static partial class CommandArgs
    {
        public struct Chat
        {
            /// <summary>
            /// The chat message.
            /// </summary>
            /// <example>help,say,exit</example>
            public string Message { get; internal set; }

            /// <summary>
            /// Requested colour.
            /// </summary>
            public Color Color { get; set; }
        }
    }

    public static partial class CommandEvents
    {
        /// <summary>
        /// Occurs when a message is received
        /// </summary>
        public static readonly HookPoint<CommandArgs.Chat> Chat = new HookPoint<CommandArgs.Chat>();
    }
}