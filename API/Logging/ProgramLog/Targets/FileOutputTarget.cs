using System;
using OTA.Misc;
using System.IO;

namespace OTA.Logging
{
    /// <summary>
    /// FileOutputTarget is physical file based LogTarget receiver that is used in addition with ProgramLog. It will create a single
    /// thread per instance and wait for a signal from ProgramLog to start writing entries to the file.
    /// </summary>
    public class FileOutputTarget : LogTarget
    {
        ProgramThread thread;
        StreamWriter file;

        public FileOutputTarget(string path)
        {
            file = new StreamWriter(path, true);
            thread = new ProgramThread("LogF", OutputThread);
            thread.IsBackground = false;
            thread.Start();
        }

        void OutputThread()
        {
            try
            {
                var list = new OutputEntry[ProgramLog.LOG_THREAD_BATCH_SIZE];

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
                        else
                            signal.WaitForIt();
                    }

                    for (int i = 0; i < items; i++)
                    {
                        var entry = list[i];
                        list[i] = default(OutputEntry);

                        if (entry.prefix != null)
                        {
                            file.Write(entry.prefix);
                        }

                        if (entry.message is string)
                        {
                            var str = (string)entry.message;
                            file.WriteLine(str);
                            file.Flush();
                        }
                        else if (entry.message is ProgressLogger)
                        {
                            var prog = (ProgressLogger)entry.message;
                            var str = "";

                            if (entry.arg == -1) // new one
                            {
                                str = String.Format("{0}: started.", prog.Message);
                            }
                            else if (entry.arg == -2) // finished one
                            {
                                str = prog.Format(true);
                            }
                            else // update
                            {
                                str = prog.Format(prog.Max != 100, entry.arg);
                            }

                            file.WriteLine(str);
                            file.Flush();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                SafeConsole.Error.WriteLine(e.ToString());
            }

            try
            {
                file.Close();
            }
            catch { }
        }
    }
}

