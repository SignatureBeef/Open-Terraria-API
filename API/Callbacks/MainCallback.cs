using System;
using OTA.Misc;
using OTA.Plugin;
using OTA.Sockets;
using OTA.Logging;
using OTA.Command;
using System.Reflection;
using System.Linq;
using System.IO;

namespace OTA.Callbacks
{
    /// <summary>
    /// The Terraia.Main (vanilla) class callbacks
    /// </summary>
    /// <remarks>
    /// This needs to be split out into other files, as this was the first ever API file.
    /// </remarks>
    public static class MainCallback
    {
        static MainCallback()
        {
            //Resolves external plugin hook assemblies. So there is no need to place the DLL beside tdsm.exe
            AppDomain.CurrentDomain.AssemblyResolve += (s, a) =>
            {
                try
                {
                    if (a.Name == "Terraria" || a.Name == "TerrariaServer")
                        return Assembly.GetEntryAssembly();
                    //                    else if (a.Name.StartsWith("System.Data.Entity"))
                    //                    {
                    //                        Console.WriteLine("FIND ENTITY");
                    //                    }

                    if (PluginManager._plugins != null)
                    {
                        var items = PluginManager._plugins.Values
                            .Where(x => x != null && x.Assembly != null && x.Assembly.FullName == a.Name)
                            .Select(x => x.Assembly)
                                .FirstOrDefault();
                        //if (items == null)
                        //{
                        //    Tools.WriteLine("[Fatal] Unable to load {0}, was this plugin removed or do you need to repatch?", a.Name);
                        //}

                        if (items != null)
                            return items;
                    }

                    //Look in libraries - assembly name must match filename
                    string filename, prefix;
                    var ix = a.Name.IndexOf(',');
                    if (ix > -1)
                    {
                        prefix = a.Name.Substring(0, ix);
                        filename = Path.Combine(Globals.LibrariesPath, prefix + ".dll");
                    }
                    else
                    {
                        prefix = a.Name;
                        filename = Path.Combine(Globals.LibrariesPath, a.Name + ".dll");
                    }

                    if (File.Exists(filename))
                    {
                        using (var ms = new MemoryStream())
                        {
                            var buff = new byte[256];
                            using (var fs = File.OpenRead(filename))
                            {
                                while (fs.Position < fs.Length)
                                {
                                    var read = fs.Read(buff, 0, buff.Length);
                                    ms.Write(buff, 0, read);
                                }
                            }
                            return Assembly.Load(ms.ToArray());
                        }
                    }

                    //Console.WriteLine(a.Name);

                    //                    //Try currently loaded
                    //                    var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                    //                            .Where(x => x.DefinedTypes.Where(y => y.Namespace == prefix).Count() > 0).ToList();
                    //
                    //                    if (null != assemblies && assemblies.Any()) return assemblies.First();  
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                return null;
            };

            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) =>
            {
                var ex = e.ExceptionObject  as Exception;
                if (ex != null)
                    ProgramLog.Log(ex, "Unhandled exception");
                else
                    ProgramLog.Error.Log("Unhandled exception encountered");
            };
        }

        /// <summary>
        /// The callback to update the console with Terraria.Main.statusText
        /// </summary>
        public static Action StatusTextChange;

        /// <summary>
        /// Game UpdateServer event. Does not occur without players.
        /// </summary>
        public static event EventHandler UpdateServer;

        /// <summary>
        /// Server tick event. Occurs without players.
        /// </summary>
        public static event EventHandler ServerTick;

        //public static bool StartEclipse;
        //public static bool StartBloodMoon;

        /// <summary>
        /// The startup call (non vanilla) for both the client and server
        /// </summary>
        static void ProgramStart()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            ProgramLog.Log("Open Terraria API build {0}{1} running on {2}",
                Globals.Build,
                Globals.PhaseToSuffix(Globals.BuildPhase),
                Tools.RuntimePlatform.ToString()
            );
            Console.ForegroundColor = Command.ConsoleSender.DefaultColour;

            Globals.Touch();
            ID.Lookup.Initialise();

            //            OTA.Data.Entity.ConnectionManager.ConnectionString = "Server=127.0.0.1;Database=tdsm;Uid=root;Pwd=;";
            //            OTA.Data.Entity.ConnectionManager.PrepareFromAssembly("MySql.Data", true);

            //            OTA.Data.Entity.ConnectionManager.ConnectionString = "Data Source=database.sqlite;Version=3;";
            //            OTA.Data.Entity.ConnectionManager.PrepareFromAssembly("System.Data.SQLite", true);

            try
            {
                var lis = new Logging.LogTraceListener();
                System.Diagnostics.Trace.Listeners.Clear();
                System.Diagnostics.Trace.Listeners.Add(lis);
                System.Diagnostics.Debug.Listeners.Clear();
                System.Diagnostics.Debug.Listeners.Add(lis);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            //This will setup the assembly resolves
            PluginManager.Initialize(Globals.PluginPath);
            PluginManager.SetHookSource(typeof(HookPoints));

            //Load the logs
            //TODO Why on earth did I put the log opening here?
            if (!ProgramLog.IsOpen)
            {
                if (!Directory.Exists(Globals.LogFolderPath)) Directory.CreateDirectory(Globals.LogFolderPath);
                var logFile = Path.Combine(Globals.LogFolderPath, "server.log");
                ProgramLog.OpenLogFile(logFile);
                ConsoleSender.DefaultColour = ConsoleColor.Gray;
            }

            //Load plugins
            PluginManager.LoadPlugins();

//            OTA.Data.Storage.IsAvailable = (bool)Assembly.GetExecutingAssembly()
//                .DefinedTypes
//                .Where(x => x.Name == "OTAContext")
//                .Select(y => y.GetMethod("HasConnection")).First().Invoke(null, null);
            OTA.Data.Storage.IsAvailable = OTA.Data.OTAContext.HasConnection();

            if (OTA.Data.Storage.IsAvailable) ProgramLog.Admin.Log("Entity framework has a registered connection.");
            else ProgramLog.Admin.Log("Entity framework has no registered connection.");


            if (OTA.Data.Storage.IsAvailable)
                using (var ctx = new OTA.Data.OTAContext())
                {
                    ctx.APIAccounts.Add(new OTA.Data.Entity.Models.APIAccount()
                        {
                            Username = "Test",
                            Password = "Testing"
                        });

                    ctx.SaveChanges();
                }

            //            if (!Permissions.PermissionsManager.IsSet)
            //            {
            //                var file = System.IO.Path.Combine(Globals.DataPath, "permissions.xml");
            //                //if (System.IO.File.Exists(file)) System.IO.File.Delete(file);
            //                if (System.IO.File.Exists(file))
            //                {
            //                    var handler = new Permissions.XmlSupplier(file);
            //                    if (handler.Load())
            //                        Permissions.PermissionsManager.SetHandler(handler);
            //                }
            //            }

            //            Web.WebServer.Start("http://localhost:8448/");
        }

        /// <summary>
        /// The first ever call from vanilla code
        /// </summary>
        /// <remarks>The callback from Terraria.Program.LaunchGame</remarks>
        /// <param name="cmd">Cmd.</param>
        public static bool OnProgramStarted(string[] cmd)
        {
            System.Threading.Thread.CurrentThread.Name = "Run";
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            //Preload our Libraries before we attempt anything
            if (Directory.Exists(Globals.LibrariesPath))
            {
                foreach (var file in Directory.GetFiles(Globals.LibrariesPath, "*.dll"))
                {
                    try
                    {
                        var data = File.ReadAllBytes(file);
                        AppDomain.CurrentDomain.Load(data);

//                        if (asm.GetName().Name == "EntityFramework")
//                        {
////                            OTA.Data.Storage.IsAvailable = true;
////                            OTA.Data.Storage.IsAvailable = Data.OTAContext.HasConnection();
//                            OTA.Data.Storage.IsAvailable = (bool)Assembly.GetExecutingAssembly()
//                                .DefinedTypes
//                                .Where(x => x.Name == "OTAContext")
//                                .Select(y => y.GetMethod("HasConnection")).First().Invoke(null, null);
//                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to load " + file);
                        Console.WriteLine(e);
                    }
                }
            }

#pragma warning disable 0162
            if (!Globals.FullAPIDefined)
            {
                Console.WriteLine("Oh noes! You're nearly there, but the OTA.dll is incorrect.");
                Console.WriteLine("If you are compiling from source you must enable the Full_API compilation flag");
                return false;
            }
#pragma warning restore 0162

            Globals.CurrentState = ServerState.PreInitialisation; //This is to be before plugins and the web server. No event required.
            ProgramStart();

            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.ProgramStart()
            {
                Arguments = cmd
            };
            HookPoints.ProgramStart.Invoke(ref ctx, ref args);

            return ctx.Result == HookResult.DEFAULT;
        }

        /// <summary>
        /// The callback from vanilla code to start shutting down
        /// </summary>
        public static void OnProgramFinished()
        {
#if ENABLE_NAT
            while (NAT.ShuttingDown)
            {
                System.Threading.Thread.Sleep(50);
            }
#endif

            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.ServerStateChange()
            {
                ServerChangeState = (Globals.CurrentState = ServerState.Stopping)
            };
            HookPoints.ServerStateChange.Invoke(ref ctx, ref args);

            PluginManager.DisablePlugins();

            //Close the logging if set
            ProgramLog.Close();
            //            if (Tools.WriteClose != null)
            //                Tools.WriteClose.Invoke();
        }

        /// <summary>
        /// The call from the XNA Game initialise
        /// </summary>
        /// <remarks>This could have been in the XNA shims, but the client uses FNA/XNA and if our client is to work they require CIL modifications</remarks>
        public static void Initialise()
        {
#if Full_API
            if (Terraria.Main.dedServ)
            {
                var ctx = new HookContext()
                {
                    Sender = HookContext.ConsoleSender
                };
                var args = new HookArgs.ServerStateChange()
                {
                    ServerChangeState = (Globals.CurrentState = ServerState.Initialising)
                };
                HookPoints.ServerStateChange.Invoke(ref ctx, ref args);
            }
#endif
        }

        public static void OnServerTick()
        {
#if Full_API
            if (Terraria.Netplay.anyClients)
            {
                for (var i = 0; i < Terraria.Netplay.Clients.Length; i++)
                {
                    var client = Terraria.Netplay.Clients[i];
                    //                if (player.active)
                    if (client != null && client.Socket != null && client.Socket is ClientConnection)
                    {
                        var conn = (client.Socket as ClientConnection);
                        if (conn != null)
                            conn.Flush();
                    }
                }
            }

            if (ServerTick != null) ServerTick(null, EventArgs.Empty);
#endif
        }

        //        private static DateTime? _lastUpdate;

        /// <summary>
        /// The call from the end of Terraria.Main.UpdateServer
        /// </summary>
        public static void UpdateServerEnd()
        {
            //            Console.WriteLine("SE");
            ///* Check tolled tasks */
            //Tasks.CheckTasks();

            if (UpdateServer != null)
                UpdateServer(null, EventArgs.Empty);

//            if (_lastUpdate != null)
//            {
//                var diff = (DateTime.Now - _lastUpdate.Value);
//                if (diff.TotalMilliseconds > 50)
//                {
//                    ProgramLog.Debug.Log("Update took {0}ms", diff.TotalMilliseconds);
//                }
//            }
//            _lastUpdate = DateTime.Now;
            
            //var ctx = new HookContext()
            //{
            //    Sender = HookContext.ConsoleSender
            //};
            //var args = new HookArgs.UpdateServer();
            //HookPoints.UpdateServer.Invoke(ref ctx, ref args);

//#if Full_API
//            try
//            {
//                if (MessageBufferCallback.PlayerCommands.Count > 0)
//                {
//                    PlayerCommandReceived cmd;
//                    if (MessageBufferCallback.PlayerCommands.TryDequeue(out cmd))
//                    {
//                        MessageBufferCallback.ProcessQueuedPlayerCommand(cmd);
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                ProgramLog.Log(e, "Exception from user chat");
//            }
//#endif
        }

        /// <summary>
        /// The call from the start of Terraria.WorldFile.loadWorld
        /// </summary>
        public static void WorldLoadBegin()
        {
            //Since this is hook is at the end of the world loading then we can clear the new progress loading
#if Full_API
            Terraria.Main.statusText = String.Empty;
#endif

            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.ServerStateChange()
            {
                ServerChangeState = (Globals.CurrentState = ServerState.WorldLoading)
            };
            HookPoints.ServerStateChange.Invoke(ref ctx, ref args);
        }

        /// <summary>
        /// The call from the end of Terraria.WorldFile.loadWorld
        /// </summary>
        public static void WorldLoadEnd()
        {
            //Since this is hook is at the end of the world loading then we can clear the new progress loading
#if Full_API
            Terraria.Main.statusText = String.Empty;
#endif

            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.ServerStateChange()
            {
                ServerChangeState = (Globals.CurrentState = ServerState.WorldLoaded)
            };
            HookPoints.ServerStateChange.Invoke(ref ctx, ref args);
        }

        /// <summary>
        /// The call from the start of Terraria.WorldGen.generateWorld
        /// </summary>
        public static void WorldGenerateBegin()
        {
            MainCallback.ResetTileArray(8400, 2400); //You make me sad Terraria.WorldGen

#if Full_API
            Terraria.Main.statusText = String.Empty;
#endif

            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.ServerStateChange()
            {
                ServerChangeState = (Globals.CurrentState = ServerState.WorldGenerating)
            };
            HookPoints.ServerStateChange.Invoke(ref ctx, ref args);
        }

        /// <summary>
        /// The call from the end of Terraria.WorldGen.generateWorld
        /// </summary>
        public static void WorldGenerateEnd()
        {
            //Since this is hook is at the end of the world loading then we can clear the new progress loading
#if Full_API
            Terraria.Main.statusText = String.Empty;
#endif

            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.ServerStateChange()
            {
                ServerChangeState = (Globals.CurrentState = ServerState.WorldGenerated)
            };
            HookPoints.ServerStateChange.Invoke(ref ctx, ref args);
        }

        //private static int _textTimeout = 0;
        /// <summary>
        /// Callback from Terraria.Main.DedServ
        /// </summary>
        public static void OnStatusTextChange()
        {
            try
            {
                /* Check tolled tasks - OnStatusTextChanged is called without clients connected */
                Tasks.CheckTasks(); //This still may not be the best place for this.

#if Full_API
                if (Terraria.Main.oldStatusText != Terraria.Main.statusText)
                {
                    if (StatusTextChange != null)
                        StatusTextChange();
                    else
                    {
                        Terraria.Main.oldStatusText = Terraria.Main.statusText;
                        ProgramLog.Log(Terraria.Main.statusText);
                    }
                    /*var ctx = new HookContext()
                {
                    Sender = HookContext.ConsoleSender
                };
                var args = new HookArgs.StatusTextChanged() { };
                HookPoints.StatusTextChanged.Invoke(ref ctx, ref args);

                if (ctx.Result == HookResult.DEFAULT)
                {
                    Terraria.Main.oldStatusText = Terraria.Main.statusText;
                    Tools.WriteLine(Terraria.Main.statusText);
                }*/
                    //_textTimeout = 0;
                }

                //else if (Terraria.Main.oldStatusText == String.Empty && Terraria.Main.statusText == String.Empty)
                //{
                //    if (_textTimeout++ > 1000)
                //    {
                //        _textTimeout = 0;
                //        Terraria.Main.statusText = String.Empty;
                //    }
                //}
#endif
            }
            catch (Exception e)
            {
                ProgramLog.Log(e, "OnStatusTextChange error");
            }
        }

        /// <summary>
        /// The call from Terraria.Main.InvasionWarning
        /// </summary>
        public static bool OnInvasionWarning()
        {
            var ctx = new HookContext();
            var args = new HookArgs.InvasionWarning();

            HookPoints.InvasionWarning.Invoke(ref ctx, ref args);
            return ctx.Result == HookResult.DEFAULT; //Continue on
        }

#if CLIENT
        /// <summary>
        /// The first call from Terraria.Main.Draw
        /// </summary>
        public static void OnDrawBegin()
        {
            var ctx = HookContext.Empty;
            var args = new HookArgs.Draw()
            {
                State = MethodState.Begin
            };

            HookPoints.Draw.Invoke(ref ctx, ref args);
        }

        /// <summary>
        /// The ending call from Terraria.Main.Draw
        /// </summary>
        public static void OnDrawEnd()
        {
            var ctx = HookContext.Empty;
            var args = new HookArgs.Draw()
            {
                State = MethodState.End
            };

            HookPoints.Draw.Invoke(ref ctx, ref args);
        }

        /// <summary>
        /// The first call from Terraria.Main.Update
        /// </summary>
        public static void OnUpdateBegin()
        {
            var ctx = HookContext.Empty;
            var args = new HookArgs.Update()
            {
                State = MethodState.Begin
            };

            HookPoints.Update.Invoke(ref ctx, ref args);
        }

        /// <summary>
        /// The end call from Terraria.Main.Update
        /// </summary>
        public static void OnUpdateEnd()
        {
            var ctx = HookContext.Empty;
            var args = new HookArgs.Update()
            {
                State = MethodState.End
            };

            HookPoints.Update.Invoke(ref ctx, ref args);
        }

        /// <summary>
        /// The first call from Terraria.Main.UpdateClient
        /// </summary>
        public static void OnUpdateClientBegin()
        {
            var ctx = HookContext.Empty;
            var args = new HookArgs.UpdateClient()
            {
                State = MethodState.Begin
            };

            HookPoints.UpdateClient.Invoke(ref ctx, ref args);
        }

        /// <summary>
        /// The end call from Terraria.Main.UpdateClient
        /// </summary>
        public static void OnUpdateClientEnd()
        {
            var ctx = HookContext.Empty;
            var args = new HookArgs.UpdateClient()
            {
                State = MethodState.End
            };

            HookPoints.UpdateClient.Invoke(ref ctx, ref args);
        }
#endif

        internal static void ResetTileArray()
        {
#if Full_API
            ResetTileArray(Terraria.Main.maxTilesX, Terraria.Main.maxTilesY);
#endif
        }

        internal static void ResetTileArray(int x, int y)
        {
#if Full_API && TileReady
            Terraria.Main.tile = new OTA.Memory.TileCollection(x, y);
#endif
        }
    }
}

