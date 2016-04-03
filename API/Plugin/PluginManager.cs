using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using OTA.Command;
using OTA.Plugin;
using OTA.Logging;
using OTA.Extensions;
using Microsoft.Xna.Framework;

#if ENTITY_FRAMEWORK_7
using Microsoft.Data.Entity;
#endif

namespace OTA.Plugin
{
    /// <summary>
    /// PluginManager class.  Handles all input/output, loading, enabling, disabling, and hook processing for plugins
    /// [TODO] Reload plugin assembly, Not enable/disable.
    /// </summary>
    public static class PluginManager
    {
        private static string _pluginPath = String.Empty;
        //private static string _libraryPath = String.Empty;

        public static Dictionary<String, BasePlugin> _plugins;
        public static List<Type> _sources = new List<Type>();

        /// <summary>
        /// Gets the plugin count.
        /// </summary>
        /// <value>The plugin count.</value>
        public static int PluginCount { get { return _plugins.Count; } }

        public static class Loaded
        {

            public static IEnumerable<String> Names
            {
                get
                {
                    return _plugins.Values.Select(x => x.Name);
                }
            }

            public static IEnumerable<String> NameAndVersions
            {
                get
                {
                    return _plugins.Values.Select(x => x.Name + ' ' + x.Version);
                }
            }
        }

        public static class Enabled
        {

            public static IEnumerable<String> Names
            {
                get
                {
                    return _plugins.Values.Where(p => p.IsEnabled).Select(x => x.Name);
                }
            }

            public static IEnumerable<String> NameAndVersions
            {
                get
                {
                    return _plugins.Values.Where(p => p.IsEnabled).Select(x => x.Name + ' ' + x.Version);
                }
            }
        }

        public static void RegisterHookSource(Type hookPoint)
        {
            lock (_sources)
                _sources.Add(hookPoint);
        }

        public static HookPoint GetHookPoint(string name)
        {
            lock (_sources)
            {
                foreach (var hookPoint in _sources)
                {
                    var fld = hookPoint.GetField(name);
                    if (fld != null)
                        return fld.GetValue(null) as HookPoint;
                }
            }

            return null;
        }

        /// <summary>
        /// Server's plugin list
        /// </summary>
        public static PluginRecordEnumerator EnumeratePluginsRecords
        {
            get
            {
                Monitor.Enter(_plugins);
                return new PluginRecordEnumerator { inner = _plugins.GetEnumerator() };
            }
        }

        /// <summary>
        /// Gets the enumeration used to enumerate over plugins
        /// </summary>
        /// <value>The enumerate plugins.</value>
        public static PluginEnumerator EnumeratePlugins
        {
            get
            {
                Monitor.Enter(_plugins);
                return new PluginEnumerator { inner = _plugins.Values.GetEnumerator() };
            }
        }

        public struct PluginRecordEnumerator : IDisposable, IEnumerator<KeyValuePair<string, BasePlugin>>, IEnumerator
        {
            internal Dictionary<string, BasePlugin>.Enumerator inner;

            public void Dispose()
            {
                inner.Dispose();
                Monitor.Exit(_plugins);
            }

            public KeyValuePair<string, BasePlugin> Current
            {
                get { return inner.Current; }
            }

            object IEnumerator.Current
            {
                get { return inner.Current; }
            }

            public PluginRecordEnumerator GetEnumerator()
            {
                return this;
            }

            public bool MoveNext()
            {
                return inner.MoveNext();
            }

            public void Reset()
            {
            }
        }

        public struct PluginEnumerator : IDisposable, IEnumerator<BasePlugin>, IEnumerator
        {
            internal Dictionary<string, BasePlugin>.ValueCollection.Enumerator inner;

            public void Dispose()
            {
                inner.Dispose();
                Monitor.Exit(_plugins);
            }

            public BasePlugin Current
            {
                get { return inner.Current; }
            }

            object IEnumerator.Current
            {
                get { return inner.Current; }
            }

            public PluginEnumerator GetEnumerator()
            {
                return this;
            }

            public bool MoveNext()
            {
                return inner.MoveNext();
            }

            public void Reset()
            {
            }
        }

        private static bool _enableLUA;

        /// <summary>
        /// PluginManager class constructor
        /// </summary>
        /// <param name="_pluginPath">Path to plugin directory</param>
        public static void Initialize(string pluginPath)
        {
            _pluginPath = pluginPath;
            //_libraryPath = libraryPath;

            _plugins = new Dictionary<String, BasePlugin>();
        }

        /// <summary>
        /// Initializes Plugin (Loads) and Checks for Out of Date Plugins.
        /// </summary>
        public static void LoadPlugins()
        {
            lock (_plugins)
            {
                LoadPluginsInternal();
                LoadScheduled(false);

                foreach (var kv in _plugins)
                {
                    var plugin = kv.Value;
                    if (plugin.OTAVersion.Minor != Globals.Version.Minor) //At this stage all plugins should be the same Major.
                    {
                        Logger.Warning($"Plugin {plugin.Name} may not work correctly as it is built for version {plugin.OTAVersion.ToString()}");
                    }
                }
            }

            var ctx = new HookContext
            {
            };

            var args = new HookArgs.PluginsLoaded
            {
            };

            HookPoints.PluginsLoaded.Invoke(ref ctx, ref args);
        }

        static void SetPluginProperty<T>(BasePlugin plugin, string name, string target)
        {
            try
            {
                var field = plugin.GetType().GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                if (field != null && field.FieldType == typeof(T))
                {
                    var prop = typeof(BasePlugin).GetProperty(target);

                    prop.SetValue(plugin, field.GetValue(null), null);
                }
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        public struct DeferredPlugin
        {
            public Assembly Assembly;
            public string FilePath;
            public string[] Dependencies;
        }

        static List<DeferredPlugin> _deferedPlugins;

        static bool CanLoadPlugin(Assembly assembly, string[] types)
        {
            foreach (var type in types)
            {
                if (_plugins.Where(x => x.Value.Assembly.GetName().Name == type).Count() == 0)
                {
                    return false;
                }
            }

            return true;
        }

        static void LoadScheduled(bool silent)
        {
            if (_deferedPlugins != null)
            {
                for (var x = 0; x < _deferedPlugins.Count; x++)
                {
                    var scheduled = _deferedPlugins[x];
                    if (CanLoadPlugin(scheduled.Assembly, scheduled.Dependencies))
                    {
                        BasePlugin plugin;
                        var res = TryLoadPluginAssembly(scheduled.Assembly, out plugin, scheduled.FilePath, false);

                        if (res == PluginLoadResult.Loaded && plugin != null)
                        {
                            plugin = PreparePlugin(plugin, scheduled.FilePath);

                            if (plugin != null)
                            {
                                //Plugin met requirements, remove it now otherwise it might initialise more than once
                                _deferedPlugins.RemoveAt(x);
                                x--;

                                if (plugin.InitializeAndHookUp())
                                {
                                    _plugins.Add(plugin.Name.ToLower().Trim(), plugin);

                                    if (plugin.EnableEarly)
                                        plugin.Enable();
                                }
                            }
                        }

                        if (plugin == null && !silent)
                        {
                            Logger.Error("Failed to load {0}.", Path.GetFileNameWithoutExtension(scheduled.FilePath));
                        }
                    }
                }
            }
        }

        static PluginLoadResult TryLoadPluginAssembly(Assembly assembly, out BasePlugin plugin, string filePath, bool schedule = true)
        {
            plugin = null;

            if (schedule)
            {
                //Check to see if the assembly contains marked plugin dependencies
                var dependencies = Attribute.GetCustomAttributes(assembly, typeof(PluginDependencyAttribute), false);
                if (dependencies != null && dependencies.Length > 0)
                {
                    var types = dependencies.Select(x => (x as PluginDependencyAttribute).AssemblyName).ToArray();

                    if (!CanLoadPlugin(assembly, types))
                    {
                        var sh = new DeferredPlugin()
                        {
                            Assembly = assembly,
                            Dependencies = types,
                            FilePath = filePath
                        };
                        if (null == _deferedPlugins)
                            _deferedPlugins = new List<DeferredPlugin>()
                            {
                                sh
                            };
                        else _deferedPlugins.Add(sh);

                        Logger.Debug("Plugin is scheduled to load pending dependency " + String.Join(",", types));
                        return PluginLoadResult.Scheduled;
                    }
                }
            }

            foreach (var type in assembly.GetTypesLoaded().Where(x => typeof(BasePlugin).IsAssignableFrom(x) && !x.IsAbstract))
            {
                plugin = CreatePluginInstance(type);
                if (plugin == null)
                {
                    throw new Exception("Could not create plugin instance");
                }
                else
                {
                    plugin.Assembly = assembly;
                    return PluginLoadResult.Loaded;
                }
            }

            return PluginLoadResult.Failed;
        }

        static BasePlugin CreatePluginInstance(System.Type type)
        {
            var plugin = (BasePlugin)Activator.CreateInstance(type);

            if (plugin == null)
                return null;

            SetPluginProperty<string>(plugin, "NAME", "Name");
            SetPluginProperty<string>(plugin, "AUTHOR", "Author");
            SetPluginProperty<string>(plugin, "DESCRIPTION", "Description");
            SetPluginProperty<string>(plugin, "VERSION", "Version");

            return plugin;
        }

        /// <summary>
        /// Load the plugin located at the specified path.
        /// This only loads one plugin.
        /// </summary>
        /// <param name="path">Path to the plugin</param>
        /// <returns>Instance of the successfully loaded plugin, otherwise null</returns>
        public static PluginLoadResult TryLoadPluginFromDLL(string path, out BasePlugin plugin)
        {
            plugin = null;
            PluginLoadResult result = PluginLoadResult.Failed;
            try
            {
                // If there are plugins in the root directory it can cause duplicate types
                // TODO: I wish this could be avoided, but they cause a debugging nightmare.
                // Need to figure out how to disable .net looking beside the launch exe.
                // Or perhaps look in Plugins & Libraries first rather than the root (probing doesn't seem to do this).
                if (!Terraria.Initializers.LaunchInitializer.HasParameter("-plugin-clean"))
                {
                    var exePath = Assembly.GetExecutingAssembly().Location;
                    foreach (var dir in new[]
                    {
                        Environment.CurrentDirectory,
                        Path.GetDirectoryName(exePath) //If the exe is started from another location
                    }.Distinct())
                    {
                        var filename = Path.GetFileName(path);
                        var fn = Path.Combine(dir, filename);
                        if (File.Exists(fn))
                        {
                            var before = Console.ForegroundColor;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write($"There is a duplicate {filename} beside {Path.GetFileName(exePath)} which typically causes issues, remove it? [Y/n]: ");
                            if (Console.ReadKey().Key == ConsoleKey.Y)
                                File.Delete(fn);
                            Console.WriteLine();
                            Console.ForegroundColor = before;
                        }
                    }
                }

                Assembly assembly = null;
                //                Type type = typeof(BasePlugin);

                using (FileStream fs = File.Open(path, FileMode.Open))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        byte[] buffer = new byte[1024];

                        int read = 0;

                        while ((read = fs.Read(buffer, 0, 1024)) > 0)
                            ms.Write(buffer, 0, read);

                        assembly = Assembly.Load(ms.ToArray());
                    }
                }

                result = TryLoadPluginAssembly(assembly, out plugin, path);
            }
            catch (Exception e)
            {
                Logger.Log(e, "Error loading plugin assembly " + path);
                result = PluginLoadResult.Failed;
            }

            return result;
        }

        static string[] GetFiles(string directory, string pattern)
        {
            return pattern.Split('|')
                .SelectMany(filter => System.IO.Directory.GetFiles(directory, filter, SearchOption.TopDirectoryOnly))
                .ToArray();
        }

        static readonly Dictionary<string, string> compilerOptions = new Dictionary<string, string>()
        {
            { "CompilerVersion", "v4.0" },
        };

        public static PluginLoadResult LoadSourcePlugin(string path, out BasePlugin plugin)
        {
            return LoadSourcePlugin(out plugin, false, path);
        }

        public static PluginLoadResult LoadSourcePlugin(out BasePlugin plugin, bool source, params string[] files)
        {
            plugin = null;
            var cp = new Microsoft.CSharp.CSharpCodeProvider(compilerOptions);
            var par = new System.CodeDom.Compiler.CompilerParameters();
            par.GenerateExecutable = false;
            par.GenerateInMemory = true;
            par.IncludeDebugInformation = true;
            //par.CompilerOptions = "/optimize";
            par.TreatWarningsAsErrors = false;

            var us = Assembly.GetExecutingAssembly();
            par.ReferencedAssemblies.Add(us.Location);

            //Add the libraries path as well as where TDSM is located
            var directory = Path.GetDirectoryName(us.Location);
            par.CompilerOptions = String.Format("/lib:\"{0}\"", Globals.LibrariesPath, directory);

            //            var execs = GetFiles(directory, "*.dll|*.exe");
            foreach (var asn in us.GetReferencedAssemblies())
            {
                var name = asn.Name;
                var execs = GetFiles(directory, name + ".dll|" + name + ".exe");
                if (execs != null && execs.Length > 0)
                    name = execs[0];
                par.ReferencedAssemblies.Add(name);
            }

            System.CodeDom.Compiler.CompilerResults result;
            if (source) result = cp.CompileAssemblyFromSource(par, files);
            else result = cp.CompileAssemblyFromFile(par, files);

            var errors = result.Errors;
            if (errors != null)
            {
                if (errors.HasErrors)
                    Logger.Error("Failed to compile source plugin:");
                foreach (System.CodeDom.Compiler.CompilerError error in errors)
                {
                    if (error.IsWarning)
                        Logger.Debug(error.ToString());
                    else
                        Logger.Error(error.ToString());
                }
                if (errors.HasErrors)
                {
                    return PluginLoadResult.Failed;
                }
            }

            return TryLoadPluginAssembly(result.CompiledAssembly, out plugin, files.First());
        }

        /// <summary>
        /// Loads the plugin from a file path.
        /// </summary>
        /// <returns>The plugin from path.</returns>
        /// <param name="file">File.</param>
        public static BasePlugin LoadPluginFromPath(string file)
        {
            FileInfo fileInfo = new FileInfo(file);
            var ext = fileInfo.Extension.ToLower();
            BasePlugin plugin = null;

            var ctx = new HookContext
            {
            };

            var args = new HookArgs.PluginLoadRequest
            {
                Path = file
            };

            HookPoints.PluginLoadRequest.Invoke(ref ctx, ref args);

            if (ctx.Result == HookResult.IGNORE)
                return null;

            if (args.LoadedPlugin == null)
            {
                if (ext == ".dll")
                {
                    Logger.Log(ProgramLog.Categories.Plugin, System.Diagnostics.TraceLevel.Info, "Loading plugin from {0}.", fileInfo.Name);
                    var res = TryLoadPluginFromDLL(file, out plugin);

                    if (res != PluginLoadResult.Scheduled && (null == plugin || res != PluginLoadResult.Loaded))
                    {
                        Logger.Error("Failed to load {0}.", fileInfo.Name);
                    }
                }
                else if (ext == ".cs")
                {
                    Logger.Log(ProgramLog.Categories.Plugin, System.Diagnostics.TraceLevel.Info, "Compiling and loading plugin from {0}.", fileInfo.Name);
                    var res = LoadSourcePlugin(file, out plugin);

                    if (res != PluginLoadResult.Scheduled && (null == plugin || res != PluginLoadResult.Loaded))
                    {
                        Logger.Error("Failed to load {0}.", fileInfo.Name);
                    }
                }
                else if (ext == ".lua")
                {
                    if (!_enableLUA)
                        _enableLUA = true;
                    Logger.Log(ProgramLog.Categories.Plugin, System.Diagnostics.TraceLevel.Info, "Loading plugin from {0}.", fileInfo.Name);
                    plugin = new LUAPlugin();
                }
            }

            if (plugin != null)
            {
                plugin = PreparePlugin(plugin, file, fileInfo);
            }

            return plugin;
        }

        static BasePlugin PreparePlugin(BasePlugin plugin, string file, FileInfo fileInfo = null)
        {
            //20151011 New versioning
            //  - Ensure they specify the attribute
            //  - If the major is not the same then it will most likey cause problems
            if (plugin.OTAVersion == null)
            {
                Logger.Log(ProgramLog.Categories.Plugin, System.Diagnostics.TraceLevel.Info, "Cannot load plugin {0} as it does not specify an OTAVersionAtrribute.", plugin.Name);
                return null;
            }
            else if (plugin.OTAVersion.Major != Globals.Version.Major)
            {
                Logger.Log(ProgramLog.Categories.Plugin, System.Diagnostics.TraceLevel.Info, "Cannot load plugin {0} as it is not supported by this version.", plugin.Name);
                return null;
            }

            plugin.Path = file;
            plugin.PathTimestamp = (fileInfo ?? new FileInfo(file)).LastWriteTimeUtc;
            if (plugin.Name == null)
                plugin.Name = Path.GetFileNameWithoutExtension(file);

            return plugin;
        }

        /// <summary>
        /// Replaces a plugin
        /// </summary>
        /// <returns><c>true</c>, if plugin was replaced, <c>false</c> otherwise.</returns>
        /// <param name="oldPlugin">Old plugin.</param>
        /// <param name="newPlugin">New plugin.</param>
        /// <param name="saveState">If set to <c>true</c> save state.</param>
        public static bool ReplacePlugin(BasePlugin oldPlugin, BasePlugin newPlugin, bool saveState = true)
        {
            lock (_plugins)
            {
                string oldName = oldPlugin.Name.ToLower().Trim();

                if (oldPlugin.ReplaceWith(newPlugin, saveState))
                {
                    string newName = newPlugin.Name.ToLower().Trim();

                    if (_plugins.ContainsKey(oldName))
                        _plugins.Remove(oldName);

                    _plugins.Add(newName, newPlugin);

                    return true;
                }
                else if (oldPlugin.IsDisposed)
                {
                    if (_plugins.ContainsKey(oldName))
                        _plugins.Remove(oldName);
                }
            }

            return false;
        }

        /// <summary>
        /// Reloads a plugin
        /// </summary>
        /// <returns>The plugin.</returns>
        /// <param name="oldPlugin">Old plugin.</param>
        /// <param name="saveState">If set to <c>true</c> save state.</param>
        public static BasePlugin ReloadPlugin(BasePlugin oldPlugin, bool saveState = true)
        {
            var fi = new FileInfo(oldPlugin.Path);

            BasePlugin newPlugin;

            if (fi.LastWriteTimeUtc > oldPlugin.PathTimestamp)
            {
                // plugin updated
                Logger.Log(ProgramLog.Categories.Plugin, System.Diagnostics.TraceLevel.Info, "Plugin {0} is being updated from file.", oldPlugin.Name);
                newPlugin = LoadPluginFromPath(oldPlugin.Path);
            }
            else
            {
                Logger.Log(ProgramLog.Categories.Plugin, System.Diagnostics.TraceLevel.Info, "Plugin {0} not updated, reinitializing.", oldPlugin.Name);
                newPlugin = CreatePluginInstance(oldPlugin.GetType());
                newPlugin.Path = oldPlugin.Path;
                newPlugin.PathTimestamp = oldPlugin.PathTimestamp;
                newPlugin.Name = oldPlugin.Name;
            }

            if (newPlugin == null)
                return oldPlugin;

            if (ReplacePlugin(oldPlugin, newPlugin, saveState))
            {
                return newPlugin;
            }
            else if (oldPlugin.IsDisposed)
            {
                return null;
            }

            return oldPlugin;
        }

        internal static void LoadPluginsInternal()
        {
            var files = Directory.GetFiles(_pluginPath);
            Array.Sort(files);

            foreach (string file in files)
            {
                var plugin = LoadPluginFromPath(file);
                if (plugin != null)
                {
                    if (plugin.InitializeAndHookUp())
                    {
                        _plugins.Add(plugin.Name.ToLower().Trim(), plugin);

                        if (plugin.EnableEarly)
                            plugin.Enable();

                        LoadScheduled(true);
                    }
                }
            }

            // Trigger OTA to start looking IDatabaseInitialisers'
            //File.Delete("database.sqlite");
            //Data.DatabaseFactory.Initialise("sqlite", "Data Source=database.sqlite");

#if ENTITY_FRAMEWORK_6
            //Init the db here so:
            //  1) Plugins can create a context
            //  2) The database is ready before and normal plugins are enabled
            try
            {
                using (var ctx = new OTA.Data.EF6.OTAContext())
                {
                    ctx.Database.Initialize(false);
                }
                OTA.Data.EF6.OTAContext.ProbeSuccess = true;
            }
            catch (Exception e)
            {
                ProgramLog.Log(e, "Database probe failed");
            }
#elif ENTITY_FRAMEWORK_7
            try
            {
                var ctx = OTA.Data.EF7.OTAContextFactory.Create();
                if (ctx != null)
                {
                    using (ctx)
                    {
                        ctx.Database.EnsureCreated();
                        //ctx.Database.Migrate();
                    }

                    //All instances of DbContext's for the current database must be created by now.
                    //Fire an event to populate the database with default values (if any)
                    foreach (var plg in Plugin.PluginManager.EnumeratePlugins)
                    {
                        plg.NotifyDatabaseCreated();
                    }
                }
            }
            catch (Exception e)
            {
                ProgramLog.Log(e, "Database probe failed");
            }
#endif

            EnablePlugins();
        }

        /// <summary>
        /// Reloads all plugins currently running on the server
        /// </summary>
        public static void ReloadPlugins(bool saveState = true)
        {
            lock (_plugins)
            {
                var list = _plugins.Values.ToArray();

                foreach (var plugin in list)
                {
                    ReloadPlugin(plugin, saveState);
                }
            }
        }

        /// <summary>
        /// Enables all plugins available to the server
        /// </summary>
        public static void EnablePlugins()
        {
            //Run the pre enable event
            //This can be used to setup the database
            foreach (var plugin in EnumeratePlugins)
            {
                plugin.RunPreEnable();
            }

            foreach (var plugin in EnumeratePlugins)
            {
                plugin.Enable();
            }
        }

        /// <summary>
        /// Disables all plugins currently running on the server
        /// </summary>
        public static void DisablePlugins()
        {
            foreach (var plugin in EnumeratePlugins)
            {
                plugin.Disable();
            }
        }

        /// <summary>
        /// Enables a plugin by name. Currently unused in core
        /// </summary>
        /// <param name="name">Plugin name</param>
        /// <returns>Returns true on plugin successfully Enabling</returns>
        public static bool EnablePlugin(string name)
        {
            lock (_plugins)
            {
                string cleanedName = name.ToLower().Trim();
                if (_plugins.ContainsKey(cleanedName))
                {
                    BasePlugin plugin = _plugins[cleanedName];
                    plugin.Enable();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Enables a plugin.
        /// </summary>
        /// <param name="name">Plugin name</param>
        /// <returns>Returns true on plugin successfully Enabling</returns>
        public static bool EnablePlugin(BasePlugin plugin)
        {
            return plugin.Enable();
        }

        /// <summary>
        /// Enables a plugin.
        /// </summary>
        /// <param name="name">Plugin name</param>
        /// <returns>Returns true on plugin successfully Enabling</returns>
        public static bool DisablePlugin(BasePlugin plugin)
        {
            return plugin.Disable();
        }

        /// <summary>
        /// Disables a plugin by name.  Currently unused in core
        /// </summary>
        /// <param name="name">Name of plugin</param>
        /// <returns>Returns true on plugin successfully Disabling</returns>
        public static bool DisablePlugin(string name)
        {
            lock (_plugins)
            {
                string cleanedName = name.ToLower().Trim();
                if (_plugins.ContainsKey(cleanedName))
                {
                    BasePlugin plugin = _plugins[cleanedName];
                    plugin.Disable();
                    return true;
                }
                return false;
            }
        }

        public static bool UnloadPlugin(BasePlugin plugin)
        {
            lock (_plugins)
            {
                bool result = plugin.Dispose();

                string cleanedName = plugin.Name.ToLower().Trim();
                if (_plugins.ContainsKey(cleanedName))
                    _plugins.Remove(cleanedName);

                return result;
            }
        }

        /// <summary>
        /// Gets plugin instance by name.
        /// </summary>
        /// <param name="name">Plugin name</param>
        /// <returns>Returns found plugin if successful, otherwise returns null</returns>
        public static BasePlugin GetPlugin(string name)
        {
            lock (_plugins)
            {
                string cleanedName = name.ToLower().Trim();
                if (_plugins.ContainsKey(cleanedName))
                {
                    return _plugins[cleanedName];
                }
                return null;
            }
        }

        internal static void NotifyWorldLoaded()
        {
            foreach (var plugin in EnumeratePlugins)
            {
                plugin.NotifyWorldLoaded();
            }
        }

        /// <summary>
        /// Registers a loaded plugin
        /// </summary>
        /// <param name="plugin">Plugin.</param>
        public static void RegisterPlugin(BasePlugin plugin)
        {
            if (!plugin.InitializeAndHookUp())
            {
                Logger.Error("Failed to initialize new plugin instance.", Color.DodgerBlue);
            }

            _plugins.Add(plugin.Name.ToLower().Trim(), plugin);

            if (!plugin.Enable())
            {
                Logger.Error("Failed to enable new plugin instance.", Color.DodgerBlue);
            }
        }

        /// <summary>
        /// Loads and initialoses a plugin
        /// </summary>
        /// <returns>The and init plugin.</returns>
        /// <param name="filePath">Path to the plugin file.</param>
        public static PluginLoadResult LoadAndInitPlugin(string filePath)
        {
            BasePlugin plugin;
            var res = TryLoadPluginFromDLL(filePath, out plugin);

            if (plugin == null || res != PluginLoadResult.Loaded)
            {
                if (res != PluginLoadResult.Scheduled)
                    Logger.Error("Plugin failed to load!");
                return res;
            }

            if (!plugin.InitializeAndHookUp())
            {
                Logger.Error("Failed to initialize plugin.");
                return PluginLoadResult.InitialiseFailed;
            }

            _plugins.Add(plugin.Name.ToLower().Trim(), plugin);

            if (!plugin.Enable())
            {
                Logger.Error("Failed to enable plugin.");
                return PluginLoadResult.EnableFailed;
            }

            return res;
        }
    }
}