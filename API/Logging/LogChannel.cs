using System;
using System.Diagnostics;

namespace OTA.Logging
{
    /// <summary>
    /// Specific log channel
    /// </summary>
    public class LogChannel
    {
        /// <summary>
        /// Defines the colour of the message
        /// </summary>
        /// <value>The color.</value>
        public ConsoleColor Color { get; private set; }

        /// <summary>
        /// This defines the display name of the channel
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// This can be a custom LogTarget which will receive messages from only this channel
        /// </summary>
        /// <value>The target.</value>
        public LogTarget Target { get; private set; }

        /// <summary>
        /// Defines the logging trace level
        /// </summary>
        /// <value>The level.</value>
        public TraceLevel Level { get; set; }

        public LogChannel(string name, ConsoleColor color, TraceLevel level)
        {
            Color = color;
            Name = name;
            Level = level;
        }

        public LogChannel(string name, LogTarget target, TraceLevel level, ConsoleColor color = ConsoleColor.Black)
        {
            Color = color;
            Name = name;

            Target = target;
            Level = level;
        }

        public void Log(string text, bool multi = false)
        {
            ProgramLog.Log(this, text, multi);
        }

        public void Log(string text)
        {
            ProgramLog.Log(this, text);
        }

        public void Log(string fmt, params object[] args)
        {
            ProgramLog.Log(this, fmt, args);
        }
    }
}

