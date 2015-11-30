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

                    if (PluginManager._plugins != null)
                    {
                        var items = PluginManager._plugins.Values
                            .Where(x => x != null && x.Assembly != null && x.Assembly.FullName == a.Name)
                            .Select(x => x.Assembly)
                                .FirstOrDefault();

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
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                return null;
            };

            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) =>
            {
                var ex = e.ExceptionObject as Exception;
                if (ex != null)
                    Logger.Log(ex, "Unhandled exception");
                else if (e.ExceptionObject != null)
                    Logger.Error("Unhandled exception: " + e.ExceptionObject.ToString());
                else
                    Logger.Error("Unhandled exception encountered");
            };
        }

        /// <summary>
        /// The startup call (non vanilla) for both the client and server
        /// </summary>
        static void ProgramStart()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Open Terraria API build {0} running on {1}",
                Globals.BuildInfo,
                Tools.RuntimePlatform.ToString()
            );
            Console.ForegroundColor = Command.ConsoleSender.DefaultColour;

            Globals.Touch();
            ID.Lookup.Initialise();

            //This will setup the assembly resolves
            PluginManager.Initialize(Globals.PluginPath);
            PluginManager.RegisterHookSource(typeof(HookPoints));

            //Load plugins
            PluginManager.LoadPlugins();

            //Initialise the default logging system if a plugin has not overridden it.
            if (Logger.UseDefaultLogger)
            {
                Logger.AddLogger(new DefaultLogger());

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

                //Prepare log for use
                if (!ProgramLog.IsFileOpen)
                {
                    if (!Directory.Exists(Globals.LogFolderPath)) Directory.CreateDirectory(Globals.LogFolderPath);
                    var logFile = Path.Combine(Globals.LogFolderPath, "server.log");

                    ProgramLog.OpenLogFile(logFile);
                    ConsoleSender.DefaultColour = ConsoleColor.Gray;
                }
            }
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

            #if SERVER
            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.ServerStateChange()
            {
                ServerChangeState = (Globals.CurrentState = ServerState.Stopping)
            };
            HookPoints.ServerStateChange.Invoke(ref ctx, ref args);
            #endif 

            PluginManager.DisablePlugins();

            //Close the logging if set
            ProgramLog.Close();
        }

        /// <summary>
        /// The call from the XNA Game initialise
        /// </summary>
        /// <remarks>This could have been in the XNA shims, but the client uses FNA/XNA and if our client is to work they require CIL modifications</remarks>
        public static void Initialise()
        {
#if Full_API && SERVER
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
#if Full_API && SERVER
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

            var ctx = HookContext.Empty;
            var args = HookArgs.ServerTick.Empty;
            HookPoints.ServerTick.Invoke(ref ctx, ref args);
#endif
        }

        #if SERVER
        //        private static DateTime? _lastUpdate;
        public static void OnUpdateServerBegin()
        {
            #if SERVER
            var ctx = HookContext.Empty;
            var args = HookArgs.ServerUpdate.Begin;
            HookPoints.ServerUpdate.Invoke(ref ctx, ref args);
            #endif
        }

        /// <summary>
        /// The call from the end of Terraria.Main.UpdateServer
        /// </summary>
        public static void OnUpdateServerEnd()
        {
            ///* Check tolled tasks */
            //Tasks.CheckTasks();

            var ctx = HookContext.Empty;
            var args = HookArgs.ServerUpdate.End;
            HookPoints.ServerUpdate.Invoke(ref ctx, ref args);
        }
        #endif

        public static void OnUpdateBegin()
        {
            var ctx = HookContext.Empty;
            var args = HookArgs.GameUpdate.Begin;
            HookPoints.GameUpdate.Invoke(ref ctx, ref args);
        }

        public static void OnUpdateEnd()
        {
            var ctx = HookContext.Empty;
            var args = HookArgs.GameUpdate.End;
            HookPoints.GameUpdate.Invoke(ref ctx, ref args);
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

            #if SERVER
            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.ServerStateChange()
            {
                ServerChangeState = (Globals.CurrentState = ServerState.WorldLoading)
            };
            HookPoints.ServerStateChange.Invoke(ref ctx, ref args);
            #endif
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

            #if SERVER
            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.ServerStateChange()
            {
                ServerChangeState = (Globals.CurrentState = ServerState.WorldLoaded)
            };
            HookPoints.ServerStateChange.Invoke(ref ctx, ref args);
            #endif
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

            #if SERVER
            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.ServerStateChange()
            {
                ServerChangeState = (Globals.CurrentState = ServerState.WorldGenerating)
            };
            HookPoints.ServerStateChange.Invoke(ref ctx, ref args);
            #endif
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

            #if SERVER
            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.ServerStateChange()
            {
                ServerChangeState = (Globals.CurrentState = ServerState.WorldGenerated)
            };
            HookPoints.ServerStateChange.Invoke(ref ctx, ref args);
            #endif
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
                //Since the patcher nurfs this, we implement it here for full and easy control over this mess
                if (Terraria.Main.oldStatusText != Terraria.Main.statusText)
                {
                    var ctx = HookContext.Empty;
                    var args = HookArgs.StatusTextChange.Empty;
                    HookPoints.StatusTextChange.Invoke(ref ctx, ref args);
                    if (ctx.Result == HookResult.DEFAULT)
                    {
                        Terraria.Main.oldStatusText = Terraria.Main.statusText;
                        Logger.Vanilla(Terraria.Main.statusText);
                    }
                }
#endif
            }
            catch (Exception e)
            {
                Logger.Log(e, "OnStatusTextChange error");
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
            
        //TODO move this out to a CommonCallback class or something?
        public static bool OnMechSpawn(float x, float y, int type, int num, int num2, int num3, MechSpawnType sender)
        {
            var ctx = new HookContext();
            var args = new HookArgs.MechSpawn()
            {
                X = x,
                Y = y,
                Type = type,
                Num = num,
                Num2 = num2,
                Num3 = num3,

                Sender = sender
            };

            HookPoints.MechSpawn.Invoke(ref ctx, ref args);
            return ctx.Result == HookResult.DEFAULT; //Continue on
        }

        public static bool OnChristmasCheck()
        {
            var ctx = new HookContext();
            var args = new HookArgs.CheckChristmas();

            HookPoints.CheckChristmas.Invoke(ref ctx, ref args);

            return ctx.Result == HookResult.DEFAULT; //Continue onto vanilla
        }

        public static bool OnHalloweenCheck()
        {
            var ctx = new HookContext();
            var args = new HookArgs.CheckHalloween();

            HookPoints.CheckHalloween.Invoke(ref ctx, ref args);

            return ctx.Result == HookResult.DEFAULT; //Continue onto vanilla
        }

        /// <summary>
        /// The request from vanilla code to start listening for commands
        /// </summary>
        public static void ListenForCommands()
        {
            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.StartCommandProcessing();
            HookPoints.StartCommandProcessing.Invoke(ref ctx, ref args);

            if (ctx.Result == HookResult.DEFAULT)
            {
                Terraria.Main.startDedInput();
            }
        }
    }

    public enum MechSpawnType : int
    {
        Item = 1,
        Npc
    }
}

