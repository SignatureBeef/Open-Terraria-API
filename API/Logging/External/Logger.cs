using System;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace OTA.Logging
{
    public static class Logger
    {
        public static class Categories
        {
            public const String Debug = "Debug";
            public const String Error = "Error";
            public const String Info = "Info";
            public const String Vanilla = "Vanilla";
            public const String Warning = "Warning";
        }

        private static readonly ConcurrentBag<ILogger> _loggers = new ConcurrentBag<ILogger>();

        public static bool HasLoggers { get; } = _loggers.Count > 0;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OTA.Logging.Logger"/> should use the default logging system.
        /// </summary>
        /// <value><c>true</c> if use default logget; otherwise, <c>false</c>.</value>
        public static bool UseDefaultLogger { get; set; } = true;

        /// <summary>
        /// Add a logger to be called for Vanilla & OTA functions
        /// </summary>
        /// <param name="logger">Logger.</param>
        public static void AddLogger(ILogger logger)
        {
            _loggers.Add(logger);
        }

        /// <summary>
        /// Log the specified category, level, message and args.
        /// </summary>
        /// <param name="category">Category.</param>
        /// <param name="level">Level.</param>
        /// <param name="message">Message.</param>
        /// <param name="args">Arguments.</param>
        public static void Log(string category, TraceLevel level, string message, params object[] args)
        {
            if (args != null && args.Length > 0) message = String.Format(message, args);

            foreach (var logger in _loggers)
                logger.Log(category, level, message);
        }

        /// <summary>
        /// Logs vanilla messages
        /// </summary>
        /// <remarks>This is typically called by vanilla Terraria code (patched in)</remarks>
        /// <param name="message">Message.</param>
        public static void Vanilla(string message)
        {
            Log(Categories.Vanilla, TraceLevel.Info, message);
        }

        /// <summary>
        /// Writes an object as a string to the vanilla log
        /// </summary>
        /// <remarks>This is typically called by vanilla Terraria code (patched in)</remarks>
        /// <param name="obj">Object.</param>
        public static void Vanilla(object obj)
        {
            Vanilla(obj.ToString());
        }

        /// <summary>
        /// Logs vanilla messages
        /// </summary>
        /// <param name="message">Message.</param>
        public static void Log(Exception exception, string message = null)
        {
            Log(Categories.Error, exception, message);
        }

        /// <summary>
        /// Logs vanilla messages
        /// </summary>
        /// <param name="message">Message.</param>
        public static void Log(string message)
        {
            Info(message);
        }

        /// <summary>
        /// Logs vanilla messages
        /// </summary>
        /// <param name="message">Message.</param>
        public static void Log(string category, Exception exception, string message = null)
        {
            if (null == message) message = exception.ToString();
            else message += Environment.NewLine + exception.ToString();
            Log(category, TraceLevel.Error, message);
        }

        /// <summary>
        /// Logs vanilla messages
        /// </summary>
        /// <param name="message">Message.</param>
        public static void Error(string message, params object[] args)
        {
            Log(Categories.Error, TraceLevel.Error, message, args);
        }

        /// <summary>
        /// Logs vanilla messages
        /// </summary>
        /// <param name="message">Message.</param>
        public static void Info(string message, params object[] args)
        {
            Log(Categories.Info, TraceLevel.Info, message, args);
        }

        /// <summary>
        /// Logs vanilla messages
        /// </summary>
        /// <param name="colour">Desired colour.</param>
        public static void Info(string message, ConsoleColor colour)
        {
            foreach (var logger in _loggers)
                logger.Log(Categories.Info, TraceLevel.Info, message, colour);
        }

        /// <summary>
        /// Logs vanilla messages
        /// </summary>
        /// <param name="message">Message.</param>
        public static void Debug(string message, params object[] args)
        {
            Log(Categories.Debug, TraceLevel.Verbose, message, args);
        }

        /// <summary>
        /// Logs vanilla messages
        /// </summary>
        /// <param name="message">Message.</param>
        public static void Warning(string message, params object[] args)
        {
            Log(Categories.Warning, TraceLevel.Warning, message, args);
        }
    }
}

