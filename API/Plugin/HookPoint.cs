using System;
using OTA;
using OTA.Command;
using OTA.Logging;
using OTA.Extensions;

#if Full_API
using Terraria;
#endif
using System.Threading;

namespace OTA.Plugin
{
    public abstract class HookPoint
    {
        public string Name { get; private set; }

        internal abstract Type DelegateType { get; }

        public HookPoint(string name)
        {
            Name = name;
        }

        public abstract int Count { get; }

        internal protected abstract void HookBase(BasePlugin plugin, Delegate callback, HookOrder order = HookOrder.NORMAL);

        internal protected void HookBase(Delegate callback, HookOrder order = HookOrder.NORMAL)
        {
            var plugin = callback.Target as BasePlugin;

            if (plugin == null)
                throw new ArgumentException("Callback doesn't point to an instance method of class BasePlugin", "callback");

            HookBase(plugin, callback, order);
        }

        internal protected abstract void Unhook(BasePlugin plugin);

        internal abstract void Replace(BasePlugin oldPlugin, BasePlugin newPlugin, Delegate callback, HookOrder order);

        //        static PropertiesFile hookprop = new PropertiesFile("hooks.properties");
        //
        //        static HookPoint()
        //        {
        //            hookprop.Load();
        //        }

        internal int currentlyExecuting;
        internal int currentlyPaused;
        internal ManualResetEvent pauseSignal;

        [ThreadStatic]
        internal static bool threadInHook;

        internal protected static object editLock = new object();
        //we use it recursively

        internal void Pause(ManualResetEvent signal) //.Set() the signal to unpause
        {
            lock (editLock)
            {
                if (pauseSignal != null)
                {
                    throw new ApplicationException("Attempt to pause hook invocation twice.");
                }

                pauseSignal = signal;
            }
        }

        internal void CancelPause()
        {
            pauseSignal = null;
        }

        internal bool AllPaused
        {
            get
            {
                var num = currentlyExecuting - currentlyPaused;
                if (num < 0)
                    Logger.Debug("Oops, currentlyExecuting < currentlyPaused!?");
                return num <= 0;
            }
        }
    }

    public class HookPoint<T> : HookPoint
    {
        struct Entry
        {
            public HookOrder order;
            public BasePlugin plugin;
            public HookAction<T> callback;
        }

        Entry[] entries = new Entry[0];

        public override int Count
        {
            get { return entries.Length; }
        }

        internal override Type DelegateType
        {
            get { return typeof(HookAction<T>); }
        }

        public HookPoint(string name)
            : base(name)
        {
        }

        public HookPoint()
            : base(NameFromType())
        {
        }

        static string NameFromType()
        {
            return typeof(T).Name.ToLower();
        }

        internal protected void Hook(BasePlugin plugin, HookAction<T> callback, HookOrder order = HookOrder.NORMAL)
        {
            lock (editLock)
            {
                var count = entries.Length;
                var copy = new Entry[count + 1];
                Array.Copy(entries, copy, count);

                copy[count] = new Entry { plugin = plugin, callback = callback, order = order };

                Array.Sort(copy, (Entry x, Entry y) => x.order.CompareTo(y.order));

                entries = copy;

                //				lock (plugin.hooks) //disabled as long as editLock is static
                {
                    plugin.hooks.Add(this);
                }
            }
        }

        internal protected void Hook(HookAction<T> callback, HookOrder order = HookOrder.NORMAL)
        {
            var plugin = callback.Target as BasePlugin;

            if (plugin == null)
                throw new ArgumentException("Callback doesn't point to an instance method of class BasePlugin", "callback");

            Hook(plugin, callback, order);
        }

        internal protected override void HookBase(BasePlugin plugin, Delegate callback, HookOrder order = HookOrder.NORMAL)
        {
            var cb = callback as HookAction<T>;

            if (cb == null)
                throw new ArgumentException(string.Format("A callback of type HookAction<{0}> expected.", typeof(T).Name), "callback");

            Hook(plugin, cb, order);
        }

        internal protected override void Unhook(BasePlugin plugin)
        {
            lock (editLock)
            {
                var count = entries.Length;

                int k = 0;
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].plugin != plugin)
                    {
                        k++;
                    }
                }

                var copy = new Entry[k];

                k = 0;
                for (int i = 0; i < count; i++)
                {
                    if (entries[i].plugin != plugin)
                    {
                        copy[k++] = entries[i];
                    }
                }

                entries = copy;

                //				lock (plugin.hooks) //disabled as long as editLock is static
                {
                    try
                    {
                        plugin.hooks.Remove(this);
                    }
                    catch (Exception e)
                    {
                        Logger.Log(e, "Exception removing hook from plugin's hook list");
                    }
                }
            }
        }

        internal override void Replace(BasePlugin oldPlugin, BasePlugin newPlugin, Delegate callback, HookOrder order)
        {
            lock (editLock)
            {
                for (int i = 0; i < entries.Length; i++)
                {
                    if (entries[i].plugin == oldPlugin)
                    {
                        entries[i] = new Entry { plugin = newPlugin, callback = (HookAction<T>)callback, order = order };
                        break;
                    }
                }
            }
        }

        public void Invoke(ref HookContext context, ref T arg)
        {
            var hooks = entries;
            var len = hooks.Length;
            bool locked;

            if (len > 0)
            {
                locked = true;
                Interlocked.Increment(ref currentlyExecuting);
            }
            else
                locked = false;

            var signal = pauseSignal;
            if (signal != null)
            {
                pauseSignal = null;
                Logger.Debug("Paused hook point {0}.", Name);
                //Interlocked.Decrement (ref currentlyExecuting);
                Interlocked.Increment(ref currentlyPaused);
                signal.WaitOne();
                Interlocked.Decrement(ref currentlyPaused);
                //Interlocked.Increment (ref currentlyExecuting);
                Logger.Debug("Unpaused hook point {0}.", Name);
            }

            try
            {
                threadInHook = true;

                for (int i = 0; i < len; i++)
                {
                    if (hooks[i].plugin.IsEnabled)
                    {
                        try
                        {
                            if (hooks[i].plugin is LUAPlugin)
                            {
                                var lp = hooks[i].plugin as LUAPlugin;
                                if (lp != null && lp.IsValid)
                                {
                                    //I wasn't going to waste more time. [TODO]
                                    var o = (object)arg;
                                    (hooks[i].plugin as LUAPlugin).Call(ref context, ref o, hooks[i].GetType().GetGenericArguments()[0].Name);
                                }
                            }
                            else
                                hooks[i].callback(ref context, ref arg);

                            if (context.Conclude)
                            {
                                return;
                            }
                        }
                        catch (NLua.Exceptions.LuaScriptException e)
                        {
                            try
                            {
                                if (e.IsNetException && e.InnerException != null)
                                {
                                    Logger.Log(e.InnerException, String.Format("Plugin {0} crashed in hook {1}", hooks[i].plugin.Name, Name));
                                }
                                else
                                {
                                    Logger.Log(e, String.Format("Plugin {0} crashed in hook {1}", hooks[i].plugin.Name, Name));
                                }
                            }
                            catch
                            {
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Log(e, String.Format("Plugin {0} crashed in hook {1}", hooks[i].plugin.Name, Name));
                        }
                    }
                }
            }
            finally
            {
                threadInHook = false;

                if (locked)
                    Interlocked.Decrement(ref currentlyExecuting);
            }
        }

        static void SortEntries(ref Entry[] array)
        {
            //var count = new int[5];

            //TODO: implement configurable sorting
        }
    }
}