using System;
using System.Collections.Generic;
using OTA.Misc;

namespace OTA.Logging
{
    public class LogTarget
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

