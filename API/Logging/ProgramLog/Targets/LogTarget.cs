using System;
using System.Collections.Generic;
using OTA.Misc;

namespace OTA.Logging
{
    /// <summary>
    /// A LogTarget is exclusive to the ProgramLog implementation and will receive queued data from it's despatch thread.
    /// It can be used to create low-level (see InteractiveLogTarget for a higher level) custom log receivers suchs as physical log files, custom debug windows or remote consoles.
    /// To register your custom implementation you must add it using the ProgramLog.AddTarget method.
    /// </summary>
    public abstract class LogTarget
    {
        protected Queue<OutputEntry> entries = new Queue<OutputEntry> (1024);
        protected ProducerConsumerSignal signal = new ProducerConsumerSignal (false);
        protected bool exit = false;

        public void Send (OutputEntry entry)
        {
            lock (entries)
            {
                entries.Enqueue (entry);
            }
            signal.Signal ();
        }

        public virtual void Close ()
        {
            exit = true;
            signal.Signal ();
        }

        protected int EntryCount ()
        {
            lock (entries)
            {
                return entries.Count;
            }
        }

        protected virtual void SetColor (ConsoleColor color)
        {
        }

        protected virtual void ResetColor ()
        {
        }
    }
}

