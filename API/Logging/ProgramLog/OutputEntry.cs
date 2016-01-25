using System;
using System.Collections.Generic;
using System.IO;
using OTA.Plugin;
using OTA.Misc;

namespace OTA.Logging
{
    /// <summary>
    /// OutputEntry defines a bunch of details that will be provided to a LogTarget via the ProgramLog depatch thread.
    /// It contains details such as the log message, colour, channel name and trace level.
    /// </summary>
    public struct OutputEntry
    {
        public string prefix;
        public string threadName;
        public string channelName;
        public System.Diagnostics.TraceLevel? traceLevel;
        public object message;
        public ConsoleColor? color;
        public int arg;
        public bool consoleOutput;

        public static bool operator ==(OutputEntry left, OutputEntry right)
        {
            return left.prefix.Equals(right.prefix) && left.message.Equals(right.message) && (left.arg == right.arg);
        }

        public static bool operator !=(OutputEntry left, OutputEntry right)
        {
            return (left.prefix != right.prefix) || (left.message != right.message) || (left.arg != right.arg);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
}

