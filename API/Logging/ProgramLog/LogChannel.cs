using System;
using System.Diagnostics;
using System.Collections.Generic;

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
        /// A flag to inform the ProgramLog thread to clean up the internals of this class when it's completed
        /// </summary>
        internal bool _closed;

        /// <summary>
        /// Defines the colour of the message
        /// </summary>
        /// <value>The color.</value>
        public ConsoleColor Color { get; set; }

        /// <summary>
        /// This defines the display name of the channel
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// This can be a custom LogTarget which will receive messages from only this channel
        /// </summary>
        /// <value>The target.</value>
        internal List<LogTarget> Targets { get; set; } = new List<LogTarget>();

        /// <summary>
        /// Defines the logging trace level
        /// </summary>
        /// <value>The level.</value>
        public TraceLevel Level { get; set; }

        /// <summary>
        /// Allows you to disable this channel being displayed in the console window
        /// </summary>
        public bool EnableConsoleOutput { get; set; } = true;

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

            Targets.Add(target);
            Level = level;
        }

        public void AddTarget(LogTarget target)
        {
            lock (Targets)
                Targets.Add(target);
        }

        public void RemoveTarget(LogTarget target)
        {
            lock (Targets)
                Targets.Remove(target);
        }

        public void Log(string text, bool multipleLines = false)
        {
            if (_closed) throw new InvalidOperationException("This channel is closed");

            ProgramLog.Log(this, text, multipleLines);
        }

        public void Log(string text)
        {
            if (_closed) throw new InvalidOperationException("This channel is closed");

            ProgramLog.Log(this, text);
        }

        public void Log(string fmt, params object[] args)
        {
            if (_closed) throw new InvalidOperationException("This channel is closed");

            ProgramLog.Log(this, fmt, args);
        }

        public void Log(Exception e)
        {
            if (_closed) throw new InvalidOperationException("This channel is closed");

            ProgramLog.Log(this, e);
        }

        public void Log(Exception e, string message)
        {
            if (_closed) throw new InvalidOperationException("This channel is closed");

            ProgramLog.Log(this, e, message);
        }

        public void Close()
        {
            if (!_closed) _closed = true;
        }
    }
}

