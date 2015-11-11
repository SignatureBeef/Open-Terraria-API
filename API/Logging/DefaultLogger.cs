using System;
using System.Diagnostics;
using System.Linq;

namespace OTA.Logging
{
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

