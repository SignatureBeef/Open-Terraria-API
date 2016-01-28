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
        private ProgramThread _thread;
        private StreamWriter _file;
        private int _maxSize;
        private bool _rotation;

        public string FilePath  { get; private set; }

        public FileOutputTarget(string path, bool rotation = true, int maxLogSize = 1024 * 1024 /*1mb*/)
        {
            this.FilePath = path;
            this._maxSize = maxLogSize;
            this._rotation = rotation;

            if (!rotation && maxLogSize > -1)
            {
                throw new NotSupportedException("Log rotation must be enabled in order for log size limits to take affect");
            }

            CreateWriter();

            _thread = new ProgramThread("LogF", OutputThread);
            _thread.IsBackground = false;
            _thread.Start();
        }

        void CreateWriter()
        {
            var path = FilePath;
            if (_rotation)
            {
                var ext = Path.GetExtension(path);

                path = path.Insert(path.Length - ext.Length, '_' + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            }

            if (File.Exists(path))
            {
                System.Threading.Thread.Sleep(1000);
                CreateWriter();
            }

            _file = new StreamWriter(path, true);
        }

        void CheckSize()
        {
            if (_maxSize > -1)
            {
                if (_file.BaseStream.Position >= _maxSize)
                {
                    _file.Close();
                    _file.Dispose();
                    CreateWriter();
                }
            }
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
                            _file.Write(entry.prefix);
                        }

                        if (entry.message is string)
                        {
                            var str = (string)entry.message;
                            _file.WriteLine(str);
                            _file.Flush();

                            CheckSize();
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

                            _file.WriteLine(str);
                            _file.Flush();

                            CheckSize();
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
                _file.Close();
            }
            catch
            {
            }
        }
    }
}

