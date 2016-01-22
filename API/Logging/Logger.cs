using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace OTA.Logging
{
    /// <summary>
    /// This class must be used by Terraria & OTAPI code, and in additon any other plugin wanting
    /// to pump log information to other plugins.
    /// It is a thread-less logger that distributes messages out to mutliple logging targets via the ILogger interface.
    /// 
    /// If you wish to receive information from OTAPI or Terraria you must implement your own ILogger
    /// and attatch it using the AddLogger method in this class. If you plan to implement your own Console printing
    /// (as like Console.WriteLine) I recommend you clear the default logger using ClearDefaultLogger().
    /// 
    /// By default OTAPI will register the DefaultLogger class which will forward information any information received via this
    /// logger class, onto to specific LogChannels by the Category value provided (see LogChannel class for more details, 
    /// but simply put a LogChannel can be used for different components of an application. e.g. Web logs, Plugin logs).
    /// If no LogChannel is found it will simply use the common ProgramLog.Log method. Either way ProgramLog will receive the information
    /// and will queue it for it's LogTarget's.
    /// A LogTarget for ProgramLog may be:
    ///     - outputting to the console via the StandardOutputTarget class
    ///     - outputting to a file system file via the FileOutputTarget class
    ///     - externally defined in another assembly. For example TDSM's remote client LogTarget.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// These are the default OTAPI categories, and any category passed through to an ILogger
        /// are not limited to these.
        /// </summary>
        public static class Categories
        {
            public const String Debug = "Debug";
            public const String Error = "Error";
            public const String Info = "Info";
            public const String Vanilla = "Vanilla";
            public const String Warning = "Warning";
        }

        /// <summary>
        /// The collection of ILoggers to receive information
        /// </summary>
        private static readonly List<ILogger> _loggers = new List<ILogger>();

        /// <summary>
        /// The synchronization object for the _loggers field
        /// </summary>
        private static readonly object _loggersLock = new object();

        public static bool HasLoggers
        { 
            get
            {
                lock (_loggersLock) return _loggers.Count > 0;
            }
        }

        /// <summary>
        /// Add a logger to be called for Vanilla & OTA functions
        /// </summary>
        /// <param name="logger">Logger.</param>
        public static void AddLogger(ILogger logger)
        {
            lock (_loggersLock)
                _loggers.Add(logger);
        }

        /// <summary>
        /// Clears all registered loggers.
        /// </summary>
        public static void ClearLoggers()
        {
            lock (_loggersLock)
                _loggers.Clear();
        }

        /// <summary>
        /// Removes the default logger implementation if it's found to be registered.
        /// This is extremely useful is you have your own console implementation
        /// </summary>
        public static void ClearDefaultLogger()
        {
            lock (_loggersLock)
            {
                for (var x = _loggers.Count - 1; x >= 0; x--)
                {
                    if (_loggers[x] is DefaultLogger)
                    {
                        _loggers.RemoveAt(x);
                    }
                }
            }
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

            lock (_loggersLock)
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
            OTA.DebugFramework.Assert.Expression(() => category == null);
            OTA.DebugFramework.Assert.Expression(() => exception == null);

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
            lock (_loggersLock)
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

