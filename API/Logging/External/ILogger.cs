using System;
using System.Diagnostics;

namespace OTA.Logging
{
    public interface ILogger
    {
        void Log(string category, TraceLevel level, string message, ConsoleColor? colour = null);
    }
}

