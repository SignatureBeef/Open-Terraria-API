using System;
using System.Threading;
using System.Collections.Generic;
using OTA.Command;

namespace OTA.Commands
{
    public static class CommandManager
    {
        public static CommandParser Parser { get; } = new CommandParser();
    }

    public class CommandManager<T>
    {
        public int runningCommands;
        public int pausedCommands;
        public ManualResetEvent commandPauseSignal;

        public Dictionary<String, T> commands { get; } = new Dictionary<String, T>();

        [ThreadStatic]
        internal static int threadInCommand;

        internal void NotifyBeforeCommand(T cmd)
        {
            Interlocked.Increment(ref runningCommands);

            var signal = commandPauseSignal;
            if (signal != null)
            {
                Interlocked.Increment(ref pausedCommands);
                signal.WaitOne();
                Interlocked.Decrement(ref pausedCommands);
            }

            threadInCommand = 1;
        }

        internal void NotifyAfterCommand(T cmd)
        {
            Interlocked.Decrement(ref runningCommands);
            threadInCommand = 0;
        }
    }
}

