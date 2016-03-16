using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using OTA.Command;
using OTA.Logging;
using OTA.Plugin;

namespace OTA.Plugin
{
    /// <summary>
    /// OTA version attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class OTAVersionAttribute : Attribute
    {
        /// <summary>
        /// [NO-LOAD] This is ticked when OTA has a major update that could potentially break all plugins.
        /// </summary>
        /// <example>Terraria.Tile swapping or underlying OTA Database changes</example>
        public int Major { get; set; }

        /// <summary>
        /// [WARNING] This is ticked with OTA makes API changes that could potentially break some plugins.
        /// </summary>
        /// <example>A new OTA build, function is renamed or arguments swapped.</example>
        public int Minor { get; set; }

        /// <summary>
        /// Used to determine the compatability with OTA
        /// </summary>
        /// <param name="major">[NO-LOAD] This is ticked when OTA has a major update that could potentially break all plugins.</param>
        /// <param name="minor">[WARNING] This is ticked with OTA makes API changes that could potentially break some plugins.</param>
        public OTAVersionAttribute(int major, int minor)
        {
            Major = major;
            Minor = minor;
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}";
        }
    }

    /// <summary>
    /// Plugin class, used as base for plugin extensions
    /// </summary>
    public abstract class BasePlugin
    {
        public OTAVersionAttribute OTAVersion
        {
            get
            {
                return GetType().GetCustomAttributes(true).Where(x => x is OTAVersionAttribute).SingleOrDefault() as OTAVersionAttribute;
            }
        }

        /// <summary>
        /// Name of the plugin
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Plugin description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Plugin author
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Plugin version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Whether to enable the plugin right after loading, so it could intercept the PluginLoadRequest hook for other plugins
        /// </summary>
        public bool EnableEarly { get; set; }

        /// <summary>
        /// Status text displayed by some of the /plugin commands
        /// </summary>
        public string Status { get; set; }

        internal string Path { get; set; }

        internal DateTime PathTimestamp { get; set; }

        internal Assembly Assembly { get; set; }
        //internal AppDomain _domain;

        public string FilePath
        {
            get { return Path; }
        }

        /// <summary>
        /// Whether this plugin is enabled or not
        /// </summary>
        public bool IsEnabled
        {
            get { return enabled == 1; }
        }

        public bool IsDisposed
        {
            get { return disposed == 1; }
        }

        internal volatile bool initialized;
        internal int disposed;
        internal int enabled;
        internal int informedOfWorld;

        internal HashSet<HookPoint> hooks = new HashSet<HookPoint>();

        internal struct HookEntry
        {
            public HookPoint hookPoint;
            public Delegate callback;
            public HookOrder order;
        }

        internal List<HookEntry> desiredHooks = new List<HookEntry>();

        protected BasePlugin()
        {
            Description = "";
            Author = "";
            Version = "";
            Name = "";
        }

        /// <summary>
        /// A callback for initializing the plugin's resources and subscribing to hooks.
        /// <param name='state'>
        /// A state object returned from OnDispose by a previous instance of the plugin, or null otherwise.
        /// </param>
        /// </summary>
        protected virtual void Initialized(object state)
        {
        }

        /// <summary>
        /// A callback for disposing of any resources held by the plugin.
        /// </summary>
        /// <param name='state'>
        /// A state object previously returned from SaveState to be disposed of as well.
        /// </param>
        protected virtual void Disposed(object state)
        {
        }

        protected virtual object SuspendedAndSaved()
        {
            return null;
        }

        protected virtual void Resumed(object state)
        {
        }

        /// <summary>
        /// Pre enabled routine, typically used when all plugins are to be loaded and not enabled.
        /// </summary>
        protected virtual void PreEnable()
        {
        }

        /// <summary>
        /// Enable routines, usually no more than enabled announcement and registering hooks
        /// </summary>
        protected virtual void Enabled()
        {
        }

        /// <summary>
        /// Disabling the plugin, usually announcement
        /// </summary>
        protected virtual void Disabled()
        {
        }

        #if ENTITY_FRAMEWORK_6
        /// <summary>
        /// Here the first ever DbContext should be called. It is used in order for the DbContext to join OTA's database model.
        /// </summary>
        protected virtual void DatabaseInitialising(System.Data.Entity.DbModelBuilder builder)
        {
        }
        #endif

        /// <summary>
        /// Called upon the database being created and ready for default values if required.
        /// </summary>
        protected virtual void DatabaseCreated()
        {
        }

        protected virtual void WorldLoaded()
        {
        }

        public void Hook<T>(HookPoint<T> hookPoint, HookAction<T> callback)
        {
            Hook<T>(hookPoint, HookOrder.NORMAL, callback);
        }

        public void Hook<T>(HookPoint<T> hookPoint, HookOrder order, HookAction<T> callback)
        {
            HookBase(hookPoint, order, callback);
        }

        public void HookBase(HookPoint hookPoint, HookOrder order, Delegate callback)
        {
            if (initialized)
                hookPoint.HookBase(this, callback);
            else
            {
                lock (desiredHooks)
                    desiredHooks.Add(new HookEntry { hookPoint = hookPoint, callback = callback, order = order });
            }
        }

        //public void HookBase(HookPoint hookPoint, HookOrder order, NLua.LuaFunction callback)
        //{

        //    if (initialized)
        //        hookPoint.HookBase(this, new HookAction<Object>((ref HookContext ctx, ref Object args) =>
        //        {
        //            callback.Call(ctx, args);
        //        }));
        //    else
        //    {
        //        lock (desiredHooks)
        //            desiredHooks.Add(new HookEntry
        //            {
        //                hookPoint = hookPoint,
        //                callback = new HookAction<Object>((ref HookContext ctx, ref Object args) =>
        //                {
        //                    callback.Call(ctx, args);
        //                }),
        //                order = order
        //            });
        //    }
        //}

        public void Unhook(HookPoint hookPoint)
        {
            if (initialized)
                hookPoint.Unhook(this);
            else
            {
                lock (desiredHooks)
                {
                    int i = 0;
                    foreach (var h in desiredHooks)
                    {
                        if (h.hookPoint == hookPoint)
                        {
                            desiredHooks.RemoveAt(i);
                            break;
                        }
                        i++;
                    }
                }
            }
        }

        public bool Enable()
        {
            if (Interlocked.CompareExchange(ref this.enabled, 1, 0) == 0)
            {
                try
                {
                    var ctx = new HookContext();
                    var args = new HookArgs.PluginEnabled
                    {
                        Plugin = this
                    };

                    HookPoints.PluginEnabled.Invoke(ref ctx, ref args);

                    Enabled();
                }
                catch (Exception e)
                {
                    Logger.Log(e, "Exception while enabling plugin " + Name);
                    return false;
                }
            }
            return true;
        }

        public bool Disable()
        {
            if (Interlocked.CompareExchange(ref this.enabled, 0, 1) == 1)
            {
                try
                {
                    Disabled();
                }
                catch (Exception e)
                {
                    Logger.Log(e, "Exception while disabling plugin " + Name);
                    return false;
                }
            }
            return true;
        }

        public bool InitializeAndHookUp(object state = null)
        {
            if (!Initialize(state))
                return false;

            foreach (var h in desiredHooks)
            {
                h.hookPoint.HookBase(this, h.callback, h.order);
            }

            return true;
        }

        public bool Initialize(object state = null)
        {
            if (initialized)
            {
                Logger.Error("Double initialize of plugin {0}.", Name);
                return true;
            }

            try
            {
                Initialized(state);
            }
            catch (Exception e)
            {
                Logger.Log(e, "Exception in initialization handler of plugin " + Name);
                return false;
            }

            if (!NotifyWorldLoaded())
                return false;

            var methods = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttributes(typeof(HookAttribute), true);
                if (attr.Length > 0)
                {
                    var ha = attr[0] as HookAttribute;
                    var hpName = method.GetParameters()[1].ParameterType.GetElementType().Name;

                    var hookPoint = PluginManager.GetHookPoint(hpName);
                    //var hookPoint = typeof(HookPoints).GetField(hpName).GetValue(null) as HookPoint;

                    if (hookPoint != null)
                    {
                        Delegate callback;
                        if (method.IsStatic) // TODO: exception handling
                            callback = Delegate.CreateDelegate(hookPoint.DelegateType, method);
                        else
                            callback = Delegate.CreateDelegate(hookPoint.DelegateType, this, method);

                        HookBase(hookPoint, ha.order, callback);
                    }
                }
            }

            initialized = true;

            return true;
        }

        internal object Suspend()
        {
            try
            {
                return SuspendedAndSaved();
            }
            catch (Exception e)
            {
                Logger.Log(e, "Exception while saving plugin state of " + Name);
                return null;
            }
        }

        internal bool Resume(object state)
        {
            try
            {
                Resumed(state);
                return true;
            }
            catch (Exception e)
            {
                Logger.Log(e, "Exception while saving plugin state of " + Name);
                return false;
            }
        }

        internal bool Dispose(object state = null)
        {
            if (Interlocked.CompareExchange(ref disposed, 1, 0) == 1)
                return true;

            var result = Disable();

            try
            {
                Disposed(state);
            }
            catch (Exception e)
            {
                Logger.Log(e, "Exception in disposal handler of plugin " + Name);
                result = false;
            }

            var ctx = new HookContext();
            var args = new HookArgs.PluginDisposed()
            {
                Plugin = this
            };
            HookPoints.PluginDisposed.Invoke(ref ctx, ref args);

            var hooks = new HookPoint[this.hooks.Count];
            this.hooks.CopyTo(hooks, 0, hooks.Length);

            foreach (var hook in hooks)
            {
                hook.Unhook(this);
            }

            if (this.hooks.Count > 0)
            {
                Logger.Warning("Failed to clean up {0} hooks of plugin {1}.", this.hooks.Count, Name);
                this.hooks.Clear();
            }

            return result;
        }

        // newPlugin should not have been initialized at this point!
        internal bool ReplaceWith(BasePlugin newPlugin, bool saveState = true)
        {
            var result = false;
            var noreturn = false;
            //            var paused = new LinkedList<HookPoint>();
            object savedState = null;

            lock (HookPoint.editLock)
            {
                Tools.NotifyAllPlayers("<Server> Reloading plugin " + Name + ", you may experience lag...", Color.White, true);

                //                var signal = new ManualResetEvent(false);

                lock (HookPoint.editLock)
                    try
                    {
                        using (this.Pause())
                        {
                            // initialize new instance with saved state
                            if (saveState)
                                savedState = Suspend();

                            Logger.Debug("Initializing new plugin instance...");
                            if (!newPlugin.Initialize(savedState))
                            {
                                if (saveState)
                                    Resume(savedState);
                                return false;
                            }

                            // point of no return, if the new plugin fails now,
                            // blame the author
                            // because it's time to dispose the old plugin
                            noreturn = true;

                            var ctx = new HookContext();
                            var args = new HookArgs.PluginReplacing()
                            {
                                OldPlugin = this,
                                NewPlugin = newPlugin
                            };
                            HookPoints.PluginReplacing.Invoke(ref ctx, ref args);

                            // replace hook subscriptions from the old plugin with new ones
                            // in the exact same spots in the invocation chains
                            lock (newPlugin.desiredHooks)
                            {
                                Logger.Debug("Replacing hooks...");

                                foreach (var h in newPlugin.desiredHooks)
                                {
                                    if (hooks.Contains(h.hookPoint))
                                    {
                                        h.hookPoint.Replace(this, newPlugin, h.callback, h.order);
                                        hooks.Remove(h.hookPoint);
                                        newPlugin.hooks.Add(h.hookPoint);
                                    }
                                    else
                                    {
                                        // this adds the hook to newPlugin.hooks
                                        h.hookPoint.HookBase(newPlugin, h.callback, h.order);
                                    }
                                }
                            }

                            Logger.Debug("Disabling old plugin instance...");
                            Disable();

                            Logger.Debug("Enabling new plugin instance...");
                            if (newPlugin.Enable())
                            {
                                result = true;
                            }
                        }
                    }
                    finally
                    {
                        Tools.NotifyAllPlayers("<Server> Done.", Color.White, true);

                        // clean up remaining hooks
                        if (noreturn)
                        {
                            Logger.Debug("Disposing of old plugin instance...");
                            Dispose();
                        }
                    }
            }

            return result;
        }

        /// <summary>
        /// Pause all hook and command invocations of this plugin.
        /// This method should only be called by the plugin itself and should always be used
        /// in a using statement.
        /// <example>
        ///     using (this.Pause ())
        ///     {
        ///         // code
        ///     }
        /// </example>
        /// </summary>
        protected PauseContext Pause()
        {
            return new PauseContext(this);
        }

        protected class PauseContext : IDisposable
        {
            BasePlugin plugin;
            ManualResetEvent signal;
            LinkedList<HookPoint> paused;

            internal PauseContext(BasePlugin plugin)
            {
                this.plugin = plugin;

                signal = new ManualResetEvent(false);
                paused = new LinkedList<HookPoint>();

                Monitor.Enter(HookPoint.editLock);

                var ctx = new HookContext();
                var args = new HookArgs.PluginPausing()
                {
                    Plugin = plugin,
                    Signal = signal
                };
                HookPoints.PluginPausing.Invoke(ref ctx, ref args);

                foreach (var hook in plugin.hooks)
                {
                    hook.Pause(signal);
                }

                Logger.Debug("Plugin {0} commands paused...", plugin.Name ?? "???");

                // wait for hooks that may have already been running to finish
                var pausing = new LinkedList<HookPoint>(plugin.hooks);

                // pausing hooks is more disruptive than pausing commands,
                // so we spinwait instead of sleeping
                var wait = new SpinWait();
                var min = HookPoint.threadInHook ? 1 : 0;
                while (pausing.Count > min)
                {
                    wait.SpinOnce();
                    var link = pausing.First;
                    while (link != null)
                    {
                        if (link.Value.AllPaused)
                        {
                            var x = link;
                            link = link.Next;
                            pausing.Remove(x);
                            paused.AddFirst(x);
                        }
                        else
                        {
                            link = link.Next;
                        }
                    }
                }

                Logger.Debug("Plugin {0} hooks paused...", plugin.Name ?? "???");
            }

            public void Dispose()
            {
                Logger.Debug("Unpausing everything related to plugin {0}...", plugin.Name ?? "???");

                var ctx = new HookContext();
                var args = new HookArgs.PluginPauseComplete()
                {
                    Plugin = plugin
                };
                HookPoints.PluginPauseComplete.Invoke(ref ctx, ref args);

                foreach (var hook in paused)
                {
                    hook.CancelPause();
                }

                signal.Set();

                Monitor.Exit(HookPoint.editLock);
            }
        }

        internal bool NotifyWorldLoaded()
        {
            //if (!Statics.WorldLoaded) return true;

            if (Interlocked.CompareExchange(ref informedOfWorld, 1, 0) != 0)
                return true;

            try
            {
                WorldLoaded();
            }
            catch (Exception e)
            {
                Logger.Log(e, "Exception in world load handler of plugin " + Name);
                return false;
            }

            return true;
        }

        #if ENTITY_FRAMEWORK_6
        internal bool NotifyDatabaseInitialising(System.Data.Entity.DbModelBuilder builder)
        {
            try
            {
                DatabaseInitialising(builder);
            }
            catch (Exception e)
            {
                ProgramLog.Log(e, "Exception in database intialised handler of plugin " + Name);
                return false;
            }

            return true;
        }
        #endif

        internal bool NotifyDatabaseCreated()
        {
            try
            {
                DatabaseCreated();
            }
            catch (Exception e)
            {
                Logger.Log(e, "Exception in database created handler of plugin " + Name);
                return false;
            }

            return true;
        }

        internal bool RunPreEnable()
        {
            try
            {
                PreEnable();
            }
            catch (Exception e)
            {
                Logger.Log(e, "Exception in PreEnable of plugin " + Name);
                return false;
            }

            return true;
        }
    }
}