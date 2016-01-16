using System;
using System.Collections.Generic;

namespace OTA.Misc
{
    /// <summary>
    /// Task manager
    /// </summary>
    public static class Tasks
    {
        static Stack<GameTask> _tasks;
        static DateTime _lastCheck;

        static Tasks()
        {
            _tasks = new Stack<GameTask>();
        }

        /// <summary>
        /// Schedule a task for triggering
        /// </summary>
        /// <param name="task">Task.</param>
        /// <param name="init">If set to <c>true</c> init.</param>
        public static void Schedule(GameTask task, bool init = true)
        {
            if (init) task.Init();
            lock (_tasks) _tasks.Push(task);
        }

        internal static void CheckTasks()
        {
            const Int32 CheckIntervalMs = 200;

            if ((DateTime.Now - _lastCheck).TotalMilliseconds >= CheckIntervalMs)
            {
                _lastCheck = DateTime.Now;
                lock (_tasks)
                {
                    for (var i = 0; i < _tasks.Count; i++)
                    {
                        GameTask task = _tasks.Pop();
                        if (task.Triggerable)
                        {
                            task.Method.BeginInvoke
                            (task,
                                (IAsyncResult res) =>
                                {
                                    task.Method.EndInvoke(res);
                                }, null
                            );
                            task.Reset();
                        }
                        _tasks.Push(task);
                    }
                }
            }
        }
    }
}
