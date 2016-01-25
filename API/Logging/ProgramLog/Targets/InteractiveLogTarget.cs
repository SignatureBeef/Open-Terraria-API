using System;
using OTA.Misc;
using System.IO;
using System.Collections.Generic;

namespace OTA.Logging
{
    /// <summary>
    /// InteractiveLogTarget defines the basis of a common LogTarget for use with the .NET TextWriters.
    /// This class can be used for writing to the Console (see Console.Out), or even to custom TextWriters 
    /// (see <see cref="OTA.Mod.Debug.ConsoleWindow.RichTextboxWriter"/>.
    /// </summary>
    public abstract class InteractiveLogTarget : LogTarget
    {
        protected ProgramThread thread;
        protected TextWriter writer;
        protected bool passExceptions = false;

        public InteractiveLogTarget(string name, TextWriter writer)
        {
            this.writer = writer;
            thread = new ProgramThread(name, OutputThread);
            thread.Start();
        }

        protected virtual void SignalIncompleteLine()
        {
        }

        protected virtual void OutputThread()
        {
            try
            {
                var list = new OutputEntry[ProgramLog.LOG_THREAD_BATCH_SIZE];
                var progs = new List<ProgressLogger>();
                var backspace = 0;

                while (exit == false || EntryCount() > 0)
                {
                    int items = 0;

                    lock (entries)
                    {
                        while (entries.Count > 0)
                        {
                            list[items++] = entries.Dequeue();
                            if (items == ProgramLog.LOG_THREAD_BATCH_SIZE) break;
                        }
                    }

                    if (items == 0)
                    {
                        if (exit)
                            break;
                        else if (progs.Count == 0)
                            signal.WaitForIt();
                    }

                    if (backspace > 0)
                    {
                        writer.Write("\r");
                    }

                    for (int i = 0; i < items; i++)
                    {
                        var entry = list[i];
                        list[i] = default(OutputEntry);

                        if(!CanWriteEntry(entry)) continue;

                        if (entry.prefix != null)
                        {
                            SetColor(ConsoleColor.Gray);
                            writer.Write(entry.prefix);
                            backspace -= entry.prefix.Length;
                        }

                        if (entry.color != null)
                            SetColor((ConsoleColor)entry.color);
                        else
                            SetColor(ConsoleColor.Gray);

                        if (entry.message is string)
                        {
                            var str = (string)entry.message;
                            writer.WriteLine(str);
                            OnMessageReceived(str);
                            backspace -= str.Length;
                        }
                        else if (entry.message is ProgressLogger)
                        {
                            var prog = (ProgressLogger)entry.message;
                            var str = "";

                            if (entry.arg == -1) // new one
                            {
                                progs.Add(prog);
                                str = String.Format("{0}: started.", prog.Message);
                            }
                            else if (entry.arg == -2) // finished one
                            {
                                progs.Remove(prog);
                                str = prog.Format(true);
                            }
                            else // update
                            {
                                str = prog.Format(prog.Max != 100, entry.arg);
                            }

                            backspace -= str.Length;
                            if (backspace <= 0)
                            {
                                writer.WriteLine(str);
                                OnMessageReceived(str);
                            }
                            else
                            {
                                writer.Write(str);
                                for (int j = 0; j < backspace; j++)
                                    writer.Write(" ");
                            }
                        }

                        ResetColor();
                    }

                    backspace = 0;
                    foreach (var prog in progs)
                    {
                        var str = String.Format("[ {0} ] ", prog.Format());
                        backspace += str.Length;
                        writer.Write(str);
                    }

                    if (progs.Count > 0)
                        SignalIncompleteLine();

                    if (backspace > 0 && EntryCount() == 0)
                    {
                        signal.WaitForIt(100);
                    }
                }
            }
            catch (Exception e)
            {
                if (!passExceptions)
                    SafeConsole.Error.WriteLine(e.ToString());
                else
                    throw;
            }
        }

        /// <summary>
        /// Occurs when a new line has been written to the writer
        /// </summary>
        protected virtual void OnMessageReceived(string message)
        {
        }

        protected virtual bool CanWriteEntry(OutputEntry entry) 
        {
            return true;
        }
    }
}

