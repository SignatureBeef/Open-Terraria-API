using System;
using System.Diagnostics;
using System.Linq;

namespace OTA.Logging
{
    /// <summary>
    /// This is the default logger used by OTAPI. Commonly it is used when no other plugins have specified a ILogger implementation.
    /// It is not guaranteed to always receive information as a plugin may optionally clear the loggers registered
    /// in the <see cref="OTA.Logging.Logger"/> class.
    /// 
    /// The purpose of this ILogger implementation is to forward logged information onto the ProgramLog for despatching
    /// to it's LogTargets.
    /// When there is a LogChannel defined in ProgramLog with the same Category passed from the Logger class, the 
    /// log information will be passed onto the specific LogChannel as it in itself serves a specific purpose.
    /// </summary>
    public class DefaultLogger : ILogger
    {
        private static readonly System.Collections.Generic.Dictionary<string, LogChannel> _channels = GetChannels();

        private static System.Collections.Generic.Dictionary<string, LogChannel> GetChannels()
        {
            return typeof(ProgramLog)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(x => x.FieldType.Name == typeof(LogChannel).Name)
                .Select(x => x.GetValue(null) as LogChannel)
                .ToDictionary(k => k.Name, v => v);
        }

        void ILogger.Log(string category, TraceLevel level, string message, ConsoleColor? colour)
        {
            if (_channels != null && _channels.ContainsKey(category))
                _channels[category].Log(message);
            else ProgramLog.Log(message);
        }
    }
}

