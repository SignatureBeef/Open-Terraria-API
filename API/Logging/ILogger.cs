using System;
using System.Diagnostics;

namespace OTA.Logging
{
    /// <summary>
    /// ILogger defines a basic log receiver that is to be registered with the <see cref="OTA.Logging.Logger"/> class.
    /// It will receive raw logs from OTAPI, Terraria code and potentially other plugins.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Occurs when Logger has a log message which needs logging
        /// </summary>
        /// <param name="category">Category. This will be "Vanilla" for Terraria specific logs, for OTAPI these will refer to values in <see cref="OTA.Logging.Logger.Categores"/></param>
        /// <param name="level">TraceLevel of the message</param>
        /// <param name="message">Message.</param>
        /// <param name="colour">Colour.</param>
        void Log(string category, TraceLevel level, string message, ConsoleColor? colour = null);
    }
}

