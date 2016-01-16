using System;

namespace OTA.Command
{
    /// <summary>
    /// A OTA exception for throwing a command error to the sender with the help usage + message
    /// </summary>
    public class CommandError : ApplicationException
    {
        /// <summary>
        /// Sends a message with the help text
        /// </summary>
        /// <param name="message">Message.</param>
        public CommandError(string message) : base(message)
        {
        }

        /// <summary>
        /// Sends a formatted message
        /// </summary>
        /// <param name="fmt">Fmt.</param>
        /// <param name="args">Arguments.</param>
        public CommandError(string fmt, params object[] args) : base(String.Format(fmt, args))
        {
        }
    }
}