using Microsoft.Xna.Framework;
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

namespace OTA
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

                foreach (var kv in _plugins)
                {
                    var plugin = kv.Value;
                    if (plugin.OTAVersion.Minor != Globals.Version.Minor) //At this stage all plugins should be the same Major.
                    {
                        ProgramLog.Error.Log($"[WARNING] Plugin {plugin.Name} may not work correctly as it is built for version {plugin.OTAVersion.ToString()}");
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

        public static void CheckPlugins()
        {
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
                ProgramLog.Log(e);
            }
        }

        static BasePlugin LoadPluginAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetTypesLoaded().Where(x => typeof(BasePlugin).IsAssignableFrom(x) && !x.IsAbstract))
            {
                var plugin = CreatePluginInstance(type);
                if (plugin == null)
                {
                    throw new Exception("Could not create plugin instance");
                }
                else
                {
                    plugin.Assembly = assembly;
                    return plugin;
                }
            }

            return null;
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
            SetPluginProperty<int>(plugin, "BUILD", "TDSMBuild");

            return plugin;
        }

        /// <summary>
        /// Load the plugin located at the specified path.
        /// This only loads one plugin.
        /// </summary>
        /// <param name="PluginPath">Path to plugin</param>
        /// <returns>Instance of the successfully loaded plugin, otherwise null</returns>
        public static BasePlugin LoadPluginFromDLL(string PluginPath)
        {
            try
            {
                Assembly assembly = null;
                //                Type type = typeof(BasePlugin);

                using (FileStream fs = File.Open(PluginPath, FileMode.Open))
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

                return LoadPluginAssembly(assembly);
            }
            catch (Exception e)
            {
                ProgramLog.Log(e, "Error loading plugin assembly " + PluginPath);
            }

            return null;
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

        public static BasePlugin LoadSourcePlugin(string path)
        {
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
            par.CompilerOptions = String.Format("/lib:{0}", Globals.LibrariesPath, directory);

            //            var execs = GetFiles(directory, "*.dll|*.exe");
            foreach (var asn in us.GetReferencedAssemblies())
            {
                var name = asn.Name;
                var execs = GetFiles(directory, name + ".dll|" + name + ".exe");
                if (execs != null && execs.Length > 0)
                    name = execs[0];
                par.ReferencedAssemblies.Add(name);
            }

            var result = cp.CompileAssemblyFromFile(par, path);

            var errors = result.Errors;
            if (errors != null)
            {
                if (errors.HasErrors)
                    ProgramLog.Error.Log("Failed to compile source plugin:");
                foreach (System.CodeDom.Compiler.CompilerError error in errors)
                {
                    if (error.IsWarning)
                        ProgramLog.BareLog(ProgramLog.Debug, error.ToString());
                    else
                        ProgramLog.BareLog(ProgramLog.Error, error.ToString());
                }
                if (errors.HasErrors)
                    return null;
            }

            return LoadPluginAssembly(result.CompiledAssembly);
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
                Path = file,
            };

            HookPoints.PluginLoadRequest.Invoke(ref ctx, ref args);

            if (ctx.Result == HookResult.IGNORE)
                return null;

            if (args.LoadedPlugin == null)
            {
                if (ext == ".dll")
                {
                    ProgramLog.Plugin.Log("Loading plugin from {0}.", fileInfo.Name);
                    plugin = LoadPluginFromDLL(file);

                    if (null == plugin)
                    {
                        ProgramLog.Error.Log("Failed to load {0}.", fileInfo.Name);
                    }
                }
                else if (ext == ".cs")
                {
                    ProgramLog.Plugin.Log("Compiling and loading plugin from {0}.", fileInfo.Name);
                    plugin = LoadSourcePlugin(file);

                    if (null == plugin)
                    {
                        ProgramLog.Error.Log("Failed to load {0}.", fileInfo.Name);
                    }
                }
                else if (ext == ".lua")
                {
                    if (!_enableLUA)
                        _enableLUA = true;
                    ProgramLog.Plugin.Log("Loading plugin from {0}.", fileInfo.Name);
                    plugin = new LUAPlugin();
                }
            }

            if (plugin != null)
            {
                //20151011 New versioning
                //  - Ensure they specify the attribute
                //  - If the major is not the same then it will most likey cause problems
                if (plugin.OTAVersion == null)
                {
                    ProgramLog.Plugin.Log("Cannot load plugin {0} as it does not specify an OTAVersionAtrribute.", plugin.Name);
                    return null;
                }
                else if (plugin.OTAVersion.Major != Globals.Version.Major)
                {
                    ProgramLog.Plugin.Log("Cannot load plugin {0} as it is not supported by this version.", plugin.Name);
                    return null;
                }

                plugin.Path = file;
                plugin.PathTimestamp = fileInfo.LastWriteTimeUtc;
                if (plugin.Name == null)
                    plugin.Name = Path.GetFileNameWithoutExtension(file);
            }

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
                ProgramLog.Plugin.Log("Plugin {0} is being updated from file.", oldPlugin.Name);
                newPlugin = LoadPluginFromPath(oldPlugin.Path);
            }
            else
            {
                ProgramLog.Plugin.Log("Plugin {0} not updated, reinitializing.", oldPlugin.Name);
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
                    }
                }
            }

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
                ProgramLog.Log(e, "Database probe failed.");
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
                ProgramLog.Plugin.Log("Failed to initialize new plugin instance.", Color.DodgerBlue);
            }

            _plugins.Add(plugin.Name.ToLower().Trim(), plugin);

            if (!plugin.Enable())
            {
                ProgramLog.Plugin.Log("Failed to enable new plugin instance.", Color.DodgerBlue);
            }
        }

        /// <summary>
        /// Loads and initialoses a plugin
        /// </summary>
        /// <returns>The and init plugin.</returns>
        /// <param name="Path">Path.</param>
        public static PluginLoadStatus LoadAndInitPlugin(string Path)
        {
            var rPlg = LoadPluginFromDLL(Path);

            if (rPlg == null)
            {
                ProgramLog.Error.Log("Plugin failed to load!");
                return PluginLoadStatus.FAIL_LOAD;
            }

            if (!rPlg.InitializeAndHookUp())
            {
                ProgramLog.Error.Log("Failed to initialize plugin.");
                return PluginLoadStatus.FAIL_INIT;
            }

            _plugins.Add(rPlg.Name.ToLower().Trim(), rPlg);

            if (!rPlg.Enable())
            {
                ProgramLog.Error.Log("Failed to enable plugin.");
                return PluginLoadStatus.FAIL_ENABLE;
            }

            return PluginLoadStatus.SUCCESS;
        }
    }

    /// <summary>
    /// Plugin load status.
    /// </summary>
    public enum PluginLoadStatus : int
    {
        FAIL_ENABLE,
        FAIL_INIT,
        FAIL_LOAD,
        SUCCESS
    }
}