using System;
using System.Diagnostics;

namespace OTA.Logging
{
    /// <summary>
    /// A LogChannel defines a log for a specific channel (or group) of actions. By default these actions can be prefixed and 
    /// coloured in the console which can assist in identifying actions such as what users have done, or even for debugging purposes.
    /// In addition to being displayed in the console the LogChannel class also allows you to specify a LogTarget 
    /// in the case you wish to write to file or some other custom target.
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

