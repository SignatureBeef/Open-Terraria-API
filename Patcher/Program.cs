using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

namespace OTA.Patcher
{
    /// <summary>
    /// Patch mode.
    /// </summary>
    public enum PatchMode
    {
        Server,
        Client
    }

    /// <summary>
    /// The startup class for the OTA Injector program.
    /// </summary>
    public class Program
    {
        #if SERVER
        static void Main(string[] args)
        {
            //By default we will patch a server
            OTAPatcher.PatchMode = PatchMode.Server;

            //Specifiy the official file name
            OTAPatcher.InputFileName = "TerrariaServer.exe";

            //Specify the output assembly[name]
            OTAPatcher.OutputName = "TerrariaServer";

            //Debugging :)
            OTAPatcher.CopyProjectFiles = true;

            //Allow auto running
            OTAPatcher.PromptToRun = true;

            //Allow the copy of the API from it's bin folder
            OTAPatcher.CopyAPI = true;

            OTAPatcher.DefaultProcess(args);
        }
        
#elif CLIENT
        static void Main(string[] args)
        {
            //By default we will patch a server
            OTAPatcher.PatchMode = PatchMode.Client;

            //Specifiy the official file name
            OTAPatcher.InputFileName = "Terraria.exe";

            //Specify the output assembly[name]
            OTAPatcher.OutputName = "Terraria";

            //Debugging :)
            OTAPatcher.CopyProjectFiles = true;

            //Allow auto running
            OTAPatcher.PromptToRun = true;

            //Allow the copy of the API from it's bin folder
            OTAPatcher.CopyAPI = true;

            OTAPatcher.DefaultProcess(args);
        }
        #endif

        //        public static void Main(string[] args)
        //        {
        //            //By default we will patch a server
        //            OTAPatcher.PatchMode = OTA.Patcher.PatchMode.Client;
        //
        //            //Specifiy the official file name
        //            OTAPatcher.InputFileName = "Terraria.exe";
        //
        //            //Specify the output assembly[name]
        //            OTAPatcher.OutputName = "Terraria";
        //
        //            OTAPatcher.CopyProjectFiles = true;
        //
        //            OTAPatcher.DefaultProcess(args);
        //        }
    }

    /// <summary>
    /// The Patcher OTA patcher API
    /// </summary>
    public static class OTAPatcher
    {
        public const String OTAGuid = "9f7bca2e-4d2e-4244-aaae-fa56ca7797ec";
        public const Int32 Build = 6;

        #if CLIENT
        private const String FolderKind = "Client";
        #elif SERVER
        private const String FolderKind = "Server";
        #endif

        /// <summary>
        /// Development tools. Copies files into the Debug folder.
        /// </summary>
        /// <param name="root">Root.</param>
        /// <param name="project">Project.</param>
        /// <param name="to">To.</param>
        /// <param name="pluginName">Plugin name.</param>
        /// <param name="debugFolder">If set to <c>true</c> debug folder.</param>
        public static void Copy(DirectoryInfo root, string project, string to, string pluginName = null, bool debugFolder = true, string projectFolder = FolderKind)
        {
            var projectBinary = (pluginName ?? project).Replace("-", ".");
            var p = debugFolder ? Path.Combine(root.FullName, project, "bin", "x86", projectFolder) : Path.Combine(root.FullName, project);
            if (!Directory.Exists(p))
                p = debugFolder ? Path.Combine(root.FullName, project, "bin", projectFolder) : Path.Combine(root.FullName, project);

            //From the project
            var dllFrom = Path.Combine(p, projectBinary + ".dll");
            var ddbFrom = Path.Combine(p, projectBinary + ".dll.mdb");
            var pdbFrom = Path.Combine(p, projectBinary + ".pdb");
            var exeFrom = Path.Combine(p, projectBinary + ".exe");

            //To the patcher
            var dllTo = Path.Combine(to, projectBinary + ".dll");
            var ddbTo = Path.Combine(to, projectBinary + ".dll.mdb");
            var pdbTo = Path.Combine(to, projectBinary + ".pdb");
            var exeTo = Path.Combine(to, projectBinary + ".exe");

            CopyDep(dllFrom, dllTo);
            CopyDep(ddbFrom, ddbTo);
            CopyDep(pdbFrom, pdbTo);
            CopyDep(exeFrom, exeTo);
        }

        /// <summary>
        /// Development tools. Ensures a directory exists.
        /// </summary>
        /// <param name="path">Path.</param>
        public static void EnsurePath(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }
        }

        /// <summary>
        /// Development tools. Copies a file.
        /// </summary>
        /// <param name="src">Source.</param>
        /// <param name="dest">Destination.</param>
        public static void CopyDep(string src, string dest)
        {
            //Remove destination
            if (File.Exists(dest))
                File.Delete(dest);

            //Copy new files

            if (File.Exists(src))
            {
                EnsurePath(dest);
                File.Copy(src, dest);
            }
            else
            {
                src = src.ToLower();
                if (File.Exists(src))
                {
                    EnsurePath(dest);
                    File.Copy(src, dest);
                }
            }
        }

        /// <summary>
        /// Gets or sets the patch mode.
        /// </summary>
        /// <value>The patch mode.</value>
        public static PatchMode PatchMode { get; set; }

        /// <summary>
        /// Gets or sets the name of the input file.
        /// </summary>
        /// <value>The name of the input file.</value>
        public static string InputFileName { get; set; }

        /// <summary>
        /// Gets or sets the name of the output assembly[name].
        /// </summary>
        /// <value>The name for the assembly.</value>
        public static string OutputName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OTA.Patcher.Program"/> copies project files.
        /// </summary>
        /// <value><c>true</c> if copy project files; otherwise, <c>false</c>.</value>
        public static bool CopyProjectFiles { get; set; }

        /// <summary>
        /// Gets or sets the OTA project directory.
        /// </summary>
        /// <value>The OTA project directory.</value>
        public static string OTAProjectDirectory { get; set; }

        /// <summary>
        /// Your solution directory if OTA fails to find Binaries
        /// </summary>
        public static string SolutionDirectory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OTA.Patcher.OTAPatcher"/> auto runs the output.
        /// </summary>
        /// <value><c>true</c> if auto run; otherwise, <c>false</c>.</value>
        public static bool PromptToRun { get; set; }

        /// <summary>
        /// Allows the copy of the API dll
        /// </summary>
        /// <value><c>true</c> if copy AP; otherwise, <c>false</c>.</value>
        public static bool CopyAPI { get; set; }

        const String DefaultLibrariesFolder = "Libraries";

        /// <summary>
        /// The path where to copy dependencies from into Binaries/Libraries
        /// </summary>
        /// <value>The libraries folder.</value>
        public static string LibrariesFolder { get; set; }

        /// <summary>
        /// When saving allow the OTA.dll references to be updated to the current Terraria assembly
        /// </summary>
        /// <value><c>true</c> if swap OTA references; otherwise, <c>false</c>.</value>
        public static bool SwapOTAReferences { get; set; }

        static OTAPatcher()
        {
            LibrariesFolder = DefaultLibrariesFolder;
        }

        /// <summary>
        /// Injector event arguments.
        /// </summary>
        public class InjectorEventArgs
        {
            public Injector Injector { get; internal set; }
        }

        /// <summary>
        /// Occurs after the patcher has just finished it's default patches
        /// </summary>
        public static event EventHandler<InjectorEventArgs> PerformPatch;

        /// <summary>
        /// Copy dependencies event arguments.
        /// </summary>
        public class CopyDependenciesEventArgs
        {
            public DirectoryInfo RootDirectory { get; internal set; }
        }

        /// <summary>
        /// Occurs when the patcher copies dependencies
        /// </summary>
        public static event EventHandler<CopyDependenciesEventArgs> CopyDependencies;

        /// <summary>
        /// Patches the server with every default working patch
        /// </summary>
        /// <param name="args">Arguments.</param>
        public static void DefaultProcess(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(Console.Title = String.Format("Open Terraria API patcher build {0}", Build));
            Console.ForegroundColor = ConsoleColor.White;
            var isMono = Type.GetType("Mono.Runtime") != null;

            var inFile = InputFileName;
            var fileName = OutputName;
            var output = fileName + ".exe";
            var patchFile = "OTA.dll";
            //            if (!File.Exists(inFile))
            //            {
            //                var bin = Path.Combine(Environment.CurrentDirectory, "bin", "x86", "Debug", inFile);
            //                if (File.Exists(bin))
            //                    inFile = bin;
            //            }
            //            if (!File.Exists(patchFile))
            //            {
            //                var bin = Path.Combine(Environment.CurrentDirectory, "bin", "x86", "Debug", patchFile);
            //                if (File.Exists(bin))
            //                    patchFile = bin;
            //            }

            DirectoryInfo root = null;

            if (!String.IsNullOrEmpty(SolutionDirectory))
            {
                root = new DirectoryInfo(SolutionDirectory);
            }

            if (null == root)
            {
                root = new DirectoryInfo(Environment.CurrentDirectory);
                while (root.GetDirectories().Where(x => x.Name == "Patcher").Count() == 0)
                {
                    if (root.Parent == null)
                    {
                        if (String.IsNullOrEmpty(OTAProjectDirectory))
                        {
                            Console.WriteLine("Failed to find root project directory");
                            Environment.Exit(1);
                            return;
                        }
                        break;
                    }
                    root = root.Parent;
                }

                if (!String.IsNullOrEmpty(OTAProjectDirectory))
                {
                    root = new DirectoryInfo(Environment.CurrentDirectory);
                    while (root.GetDirectories().Where(x => x.Name == OTAProjectDirectory).Count() == 0)
                    {
                        if (root.Parent == null)
                        {
                            Console.WriteLine("Failed to find root project directory using hint: " + OTAProjectDirectory);
                            Environment.Exit(1);
                            return;
                        }
                        root = root.Parent;
                    }
                }
            }

            Console.WriteLine("Root directory: " + root.FullName);

            if (PatchMode == PatchMode.Server)
            {
                if (CopyProjectFiles)
                {
                    if (CopyAPI)
                    {
                        Copy(root, "API", Environment.CurrentDirectory, "OTA", true);
                        Copy(root, "API", Path.Combine(Environment.CurrentDirectory, LibrariesFolder), "Microsoft.Owin.Diagnostics", true);
                        Copy(root, "API", Path.Combine(Environment.CurrentDirectory, LibrariesFolder), "Microsoft.Owin", true);
                        Copy(root, "API", Path.Combine(Environment.CurrentDirectory, LibrariesFolder), "Microsoft.Owin.FileSystems", true);
                        Copy(root, "API", Path.Combine(Environment.CurrentDirectory, LibrariesFolder), "Microsoft.Owin.Host.HttpListener", true);
                        Copy(root, "API", Path.Combine(Environment.CurrentDirectory, LibrariesFolder), "Microsoft.Owin.Hosting", true);
                        Copy(root, "API", Path.Combine(Environment.CurrentDirectory, LibrariesFolder), "Microsoft.Owin.Security", true);
                        Copy(root, "API", Path.Combine(Environment.CurrentDirectory, LibrariesFolder), "Microsoft.Owin.Security.OAuth", true);
                        Copy(root, "API", Path.Combine(Environment.CurrentDirectory, LibrariesFolder), "Microsoft.Owin.StaticFiles", true);
                        Copy(root, "API", Path.Combine(Environment.CurrentDirectory, LibrariesFolder), "Newtonsoft.Json", true);
                        Copy(root, "API", Path.Combine(Environment.CurrentDirectory, LibrariesFolder), "Owin", true);
                        Copy(root, "API", Path.Combine(Environment.CurrentDirectory, LibrariesFolder), "System.Net.Http.Formatting", true);
                        Copy(root, "API", Path.Combine(Environment.CurrentDirectory, LibrariesFolder), "System.Web.Http", true);
                        Copy(root, "API", Path.Combine(Environment.CurrentDirectory, LibrariesFolder), "System.Web.Http.Owin", true);
                        Copy(root, "API", Path.Combine(Environment.CurrentDirectory, LibrariesFolder), "EntityFramework", true);
                        Copy(root, "API", Path.Combine(Environment.CurrentDirectory, LibrariesFolder), "EntityFramework.SqlServer", true);
                        Copy(root, "API", Path.Combine(Environment.CurrentDirectory, LibrariesFolder), "Microsoft.AspNet.Identity.Core", true);
                        Copy(root, "API", Path.Combine(Environment.CurrentDirectory, LibrariesFolder), "Microsoft.AspNet.Identity.EntityFramework", true);
//                        Copy(root, "API", Path.Combine(Environment.CurrentDirectory, LibrariesFolder), "System.Data.SQLite", true);
//                        Copy(root, "API", Path.Combine(Environment.CurrentDirectory, LibrariesFolder), "System.Data.SQLite.EF6", true);

                        if (!String.IsNullOrEmpty(OTAProjectDirectory))
                        {
                            Copy(new DirectoryInfo(Path.Combine(root.FullName, OTAProjectDirectory)), "API", Environment.CurrentDirectory, "OTA", true);
                        }
                    }

                    //            Copy(root, "TDSM-Core", Path.Combine(Environment.CurrentDirectory, "Plugins"));
                    //            Copy(root, "Binaries", Path.Combine(Environment.CurrentDirectory), "TDSM.API");
                    //Copy (root, "Restrict", Path.Combine (Environment.CurrentDirectory, "Plugins"), "RestrictPlugin");
                    Copy(root, "External", Path.Combine(Environment.CurrentDirectory, LibrariesFolder), "KopiLua", false);
                    Copy(root, "External", Path.Combine(Environment.CurrentDirectory, LibrariesFolder), "NLua", false);
                    Copy(root, "External", Path.Combine(Environment.CurrentDirectory, LibrariesFolder), "ICSharpCode.SharpZipLib", false);
                    Copy(root, "Official", Environment.CurrentDirectory, "TerrariaServer", false);
                    //            Copy(root, "tdsm-core", Path.Combine(Environment.CurrentDirectory, LibrariesFolder), "Newtonsoft.Json", true);
                    //            Copy(root, "tdsm-web", Path.Combine(Environment.CurrentDirectory, "Plugins"), "tdsm-web", true);
                    //            Copy(root, "tdsm-mysql-connector", Path.Combine(Environment.CurrentDirectory, "Plugins"), "tdsm-mysql-connector", true);
                    //            Copy(root, "tdsm-sqlite-connector", Path.Combine(Environment.CurrentDirectory, "Plugins"), "tdsm-sqlite-connector", true);

                    if (CopyDependencies != null)
                        CopyDependencies.Invoke(null, new CopyDependenciesEventArgs()
                            {
                                RootDirectory = root
                            });
                }

                if (!File.Exists(inFile))
                {
                    //Download the supported vanilla software from our GitHub repo
                    Console.WriteLine("The original Re-Logic TerrariaServer.exe is missing, download? [Y/n]");
                    if (Console.ReadKey(true).Key == ConsoleKey.Y)
                    {
                        //TODO add throbber
                        Console.WriteLine("Download started...");
                        const String Url = "https://github.com/DeathCradle/Open-Terraria-API/raw/master/Official/TerrariaServer.exe";
                        using (var wc = new System.Net.WebClient())
                        {
                            var started = DateTime.Now;
                            try
                            {
                                wc.DownloadFile(Url, inFile);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                Console.WriteLine("Press any key to exit...");
                                Console.ReadKey(true);
                                return;
                            }
                            var duration = DateTime.Now - started;
                            Console.WriteLine("Download completed in {0:c}", duration);
                        }
                    }
                    else
                        return;
                }

            }
            else if (PatchMode == PatchMode.Client)
            {
                if (CopyProjectFiles)
                {
                    if (CopyAPI)
                    {
                        Copy(root, "API", Environment.CurrentDirectory, "OTA", true);

                        if (!String.IsNullOrEmpty(OTAProjectDirectory))
                        {
                            Copy(new DirectoryInfo(Path.Combine(root.FullName, OTAProjectDirectory)), "API", Environment.CurrentDirectory, "OTA", true);
                        }
                    }

                    if (CopyDependencies != null)
                        CopyDependencies.Invoke(null, new CopyDependenciesEventArgs()
                            {
                                RootDirectory = root
                            });
                }
            }

            var patcher = new Injector(inFile, patchFile);

            if (PatchMode == PatchMode.Server)
            {
                var noVersionCheck = args != null && args.Where(x => x.ToLower() == "-nover").Count() > 0;
                if (noVersionCheck != true)
                {
                    var vers = patcher.GetAssemblyVersion();
                    if (vers != APIWrapper.TerrariaVersion)
                    {
                        Console.WriteLine("This patcher only supports Terraria {0}, but we have detected something else {1}.", APIWrapper.TerrariaVersion, vers);
                        Console.Write("There's a high chance this will fail, continue? (y/n)");
                        if (Console.ReadKey(true).Key != ConsoleKey.Y)
                            return;
                        Console.WriteLine();
                    }
                }

                Console.Write("Opening up classes for API usage...");
                patcher.MakeTypesPublic(true);
                patcher.MakeEverythingAccessible();
                Console.Write("Ok\nHooking command line...");
                patcher.PatchCommandLine();
                Console.Write("Ok\nHooking senders...");
                patcher.HookSenders();
                Console.Write("Ok\nRemoving console handlers...");
                patcher.RemoveConsoleHandler();
                Console.Write("Ok\nRemoving mono incompatible code...");
                patcher.SwapProcessPriority();
                Console.Write("Ok\nSkipping sysmenus functions...");
                patcher.SkipMenu(PatchMode);
                Console.Write("Ok\nPatching save paths...");
                patcher.FixSavePath();
                Console.Write("Ok\nHooking receive buffer...");
                patcher.HookMessageBuffer();
                Console.Write("Ok\nPatching XNA...");
                patcher.PatchXNA(true);
                Console.Write("Ok\nPatching Steam...");
                patcher.PatchSteam();
                Console.Write("Ok\nHooking start...");
                patcher.HookProgramStart(PatchMode);
                Console.Write("Ok\nHooking initialise...");
                patcher.HookInitialise();
                patcher.HookNetplayInitialise();
                Console.Write("Ok\nHooking into world events...");
                patcher.HookWorldEvents();
                Console.Write("Ok\nHooking statusText...");
                patcher.HookStatusText();
                Console.Write("Ok\nHooking NetMessage...");
                patcher.HookNetMessage();
                Console.Write("Ok\nHooking Server events...");
                patcher.HookUpdateServer();
                patcher.HookDedServEnd();
                Console.Write("Ok\nHooking NPC Spawning...");
                patcher.HookNPCSpawning();
                Console.Write("Ok\nHooking config...");
                patcher.HookConfig();
                Console.Write("Ok\nFixing statusText...");
                patcher.FixStatusTexts();
                Console.Write("Ok\nHooking invasions...");
                patcher.HookInvasions();
                patcher.HookInvasionWarning();
//                Console.Write("Ok\nEnabling rain...");
//                patcher.EnableRaining();

                Console.Write("Ok\nFixing world removal...");
                patcher.PathFileIO();
                Console.Write("Ok\nRouting network message validity...");
                patcher.HookValidPacketState();

                Console.Write("Ok\nRemoving port forwarding functionality...");
                patcher.FixNetplay();
                Console.Write("Ok\nFixing NPC AI crashes...");
                patcher.FixRandomErrors();
                //            patcher.DetectMissingXNA();

//                patcher.HookWorldFile_DEBUG();

                Console.Write("Ok\n");
                patcher.InjectHooks<ServerHookAttribute>();

                Console.Write("Ok\nUpdating to .NET v4.5.1...");
                patcher.SwitchFramework("4.5.1");
                Console.Write("Ok\nPatching Newtonsoft.Json...");
                patcher.PatchJSON();

                Console.Write("Ok\nPutting Terraria on a diet...");
                patcher.SwapToVanillaTile(); //Holy shit batman! it works
                patcher.InjectTileSet();

                Console.Write("Ok\n");

                if (PerformPatch != null)
                    PerformPatch.Invoke(null, new InjectorEventArgs()
                        {
                            Injector = patcher
                        });

                //TODO repace Terraria's Console.SetTitles

            }
            else if (PatchMode == PatchMode.Client)
            {
                Console.Write("Hooking start...");
                patcher.HookProgramStart(PatchMode);
                Console.Write("Opening up classes for API usage...");
                patcher.MakeTypesPublic(true);
                patcher.MakeEverythingAccessible();
                Console.Write("Ok\nHooking senders...");
                patcher.HookSenders();
//                Console.Write("Ok\nPutting Terraria on a diet...");
//                patcher.SwapToVanillaTile(); //Holy shit batman! it works
//                patcher.InjectTileSet();

                Console.Write("Ok\nInjecting hooks");
                patcher.InjectHooks<ClientHookAttribute>();
                Console.Write("Ok\n");

                if (PerformPatch != null)
                    PerformPatch.Invoke(null, new InjectorEventArgs()
                        {
                            Injector = patcher
                        });
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("Saving to {0}...", output);
            patcher.Save(PatchMode, output, Build, OTAGuid, fileName, SwapOTAReferences);
            patcher.Dispose();
            Console.WriteLine("Ok");

            //            #if SERVER


            //#if DEBUG && SERVER
            if (CopyProjectFiles)
            {
                Console.Write("Updating Binaries folder...");
                UpdateBinaries();
                Console.WriteLine("Ok");
            }
            //#endif

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("You may now run {0} as you would normally.", output);

            if (PromptToRun)
            {
                var noRun = args != null && args.Where(x => x.ToLower() == "-norun").Count() > 0;
                if (!noRun)
                {
                    PromptUserToRun(output, isMono);
                }
            }
        }

        /// <summary>
        /// Prompts the user to run.
        /// </summary>
        /// <param name="output">Output.</param>
        /// <param name="isMono">If set to <c>true</c> is mono.</param>
        public static void PromptUserToRun(string output, bool isMono)
        {
            Console.WriteLine("Press [y] to run {0}, any other key will exit . . .", output);
            if (Console.ReadKey(true).Key == ConsoleKey.Y)
            {
                Run(output, isMono);
            }
        }

        /// <summary>
        /// Run the specified patched file.
        /// </summary>
        /// <param name="file">File.</param>
        /// <param name="isMono">If set to <c>true</c> is mono.</param>
        /// <param name="configFile">Config file.</param>
        public static void Run(string file, bool isMono, string configFile = "server.config")
        {
            if (!isMono)
            {
                if (PatchMode == PatchMode.Server)
                {
                    if (File.Exists(configFile))
                        System.Diagnostics.Process.Start(file, "-config " + configFile);
                    else
                        System.Diagnostics.Process.Start(file);
                }
                else if (PatchMode == PatchMode.Client)
                {
                    System.Diagnostics.Process.Start(file);
                }
            }
            else
            {
                Console.Clear();

                var domain = AppDomain.CreateDomain("ota_exec");

                using (var ms = new MemoryStream())
                {
                    using (var fs = File.OpenRead(file))
                    {
                        var buff = new byte[256];
                        while (fs.Position < fs.Length)
                        {
                            var task = fs.Read(buff, 0, buff.Length);
                            ms.Write(buff, 0, task);
                        }
                    }

                    ms.Seek(0L, SeekOrigin.Begin);
//                    var asm = System.Reflection.Assembly.Load(ms.ToArray());
                    var asm = domain.Load(ms.ToArray());
                    try
                    {
                        if (PatchMode == PatchMode.Server)
                        {
                            if (File.Exists(configFile))
                                asm.EntryPoint.Invoke(null, new object[]
                                    {
                                        new string[] { "-config", configFile, "-noupnp", "-heartbeat", "false" }
                                    });
                            else
                                asm.EntryPoint.Invoke(null, new object[]{ new string[] { } });

                        }
                        else if (PatchMode == PatchMode.Client)
                        {
                            asm.EntryPoint.Invoke(null, new object[]{ new string[] { } });
                        }
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(e);
                    }
                }
            }
        }

        /// <summary>
        /// Config modification.
        /// </summary>
        public class ConfigModification
        {
            public string OfficialLinePrefix { get; set; }

            public int Offset { get; set; }

            public string[] Modifications { get; set; }
        }

        /// <summary>
        /// Patchs the config with the modifications file.
        /// </summary>
        /// <returns>The config.</returns>
        /// <param name="input">Input.</param>
        /// <param name="targetFile">Target file.</param>
        public static string PatchConfig(string[] input, string targetFile = "serverconfig.mods.json")
        {
            var lines = new List<String>(input);

            try
            {
                var mt = File.ReadAllText(targetFile);
                var mods = Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigModification[]>(mt);

                if (mods != null)
                {
                    foreach (var mod in mods)
                    {
                        //Get indicies
                        var indicies = new List<Int32>();
                        for (var x = 0; x < lines.Count; x++)
                        {
                            if (lines[x].StartsWith(mod.OfficialLinePrefix))
                            {
                                indicies.Add(x);
                            }
                        }

                        var extra = String.Join(Environment.NewLine, mod.Modifications);
                        foreach (var index in indicies)
                        {
                            if (mod.Offset == 0)
                            {
                                lines[index] = extra;
                            }
                            else if (mod.Offset == -1)
                            {
                                lines.Insert(index + (mod.Offset + 1), extra);
                            }
                            else if (mod.Offset == 1)
                            {
                                lines.Insert(index + mod.Offset, extra);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Failed to patch config: {0}", e);
            }

            return String.Join(Environment.NewLine, lines.ToArray());
        }

        /// <summary>
        /// Gets the binaries folder.
        /// </summary>
        /// <returns>The binaries folder.</returns>
        public static DirectoryInfo GetBinariesFolder()
        {
            var pathToBinaries = new DirectoryInfo(Environment.CurrentDirectory);
            while (!Directory.Exists(Path.Combine(pathToBinaries.FullName, "Binaries")))
            {
                pathToBinaries = pathToBinaries.Parent;
            }
            return new DirectoryInfo(Path.Combine(pathToBinaries.FullName, "Binaries"));
        }

        /// <summary>
        /// Generates the output config.
        /// </summary>
        /// <param name="official">Official.</param>
        /// <param name="additional">Additional.</param>
        /// <param name="output">Output.</param>
        public static void GenerateConfig(string official = "serverconfig.txt", string additional = "additional.config", string output = "server.config")
        {
            var pathToBinaries = GetBinariesFolder();
            if (!pathToBinaries.Exists)
            {
                Console.WriteLine("Failed to copy to binaries.");
                return;
            }

            var outputPath = Path.Combine(pathToBinaries.FullName, output);

            if (File.Exists(output))
                File.Delete(output);
            if (File.Exists(outputPath))
                File.Delete(outputPath);

            var cfg = File.ReadAllLines(official);
            var contents = PatchConfig(cfg);
            contents += Environment.NewLine;
            contents += File.ReadAllText(additional);

            File.WriteAllText(output, contents);
            File.WriteAllText(outputPath, contents);
        }

        /// <summary>
        /// The list of binaries files to be updated
        /// </summary>
        public static List<String> BinariesFiles = new List<String>(new string[]
            {
                "OTA.dll",
                "OTA.dll.mdb",
                "OTA.pdb",
                LibrariesFolder + Path.DirectorySeparatorChar + "Newtonsoft.Json.dll",
                LibrariesFolder + Path.DirectorySeparatorChar + "Newtonsoft.Json.pdb",
                LibrariesFolder + Path.DirectorySeparatorChar + "NLua.dll",
                "OTA.Patcher.exe",
                "OTA.Patcher.pdb",
                "OTA.Patcher.mdb",
                //                "Vestris.ResourceLib.dll",
                LibrariesFolder + Path.DirectorySeparatorChar + "KopiLua.dll",
//                LibrariesFolder + Path.DirectorySeparatorChar + "ICSharpCode.SharpZipLib.dll",
                LibrariesFolder + Path.DirectorySeparatorChar + "Open.Nat.dll",

                "Mono.Cecil.dll",
                "Mono.Cecil.pdb",

                "start-server.bat",
                "start-server.sh",
                "start-server.cmd",

                //OWIN
                LibrariesFolder + Path.DirectorySeparatorChar + "Microsoft.Owin.Diagnostics.dll",
                LibrariesFolder + Path.DirectorySeparatorChar + "Microsoft.Owin.dll",
                LibrariesFolder + Path.DirectorySeparatorChar + "Microsoft.Owin.FileSystems.dll",
                LibrariesFolder + Path.DirectorySeparatorChar + "Microsoft.Owin.Host.HttpListener.dll",
                LibrariesFolder + Path.DirectorySeparatorChar + "Microsoft.Owin.Hosting.dll",
                LibrariesFolder + Path.DirectorySeparatorChar + "Microsoft.Owin.Security.dll",
                LibrariesFolder + Path.DirectorySeparatorChar + "Microsoft.Owin.Security.OAuth.dll",
                LibrariesFolder + Path.DirectorySeparatorChar + "Microsoft.Owin.StaticFiles.dll",
                LibrariesFolder + Path.DirectorySeparatorChar + "Owin.dll",
                LibrariesFolder + Path.DirectorySeparatorChar + "System.Net.Http.Formatting.dll",
                LibrariesFolder + Path.DirectorySeparatorChar + "System.Web.Http.dll",
                LibrariesFolder + Path.DirectorySeparatorChar + "System.Web.Http.Owin.dll",
                LibrariesFolder + Path.DirectorySeparatorChar + "EntityFramework.dll",
                LibrariesFolder + Path.DirectorySeparatorChar + "Microsoft.AspNet.Identity.Core.dll",
                LibrariesFolder + Path.DirectorySeparatorChar + "Microsoft.AspNet.Identity.EntityFramework.dll",
                LibrariesFolder + Path.DirectorySeparatorChar + "System.Data.SQLite.dll",
                LibrariesFolder + Path.DirectorySeparatorChar + "System.Data.SQLite.EF6.dll"
            });

        /// <summary>
        /// Updates the binaries.
        /// </summary>
        public static void UpdateBinaries()
        {
            var pathToBinaries = GetBinariesFolder();
            if (!pathToBinaries.Exists)
            {
                Console.WriteLine("Failed to copy to binaries.");
                return;
            }

            BinariesFiles.Add(OutputName + ".exe");
            BinariesFiles.Add(OutputName + ".exe.config");

            foreach (var rel in BinariesFiles)
            {
                if (File.Exists(rel))
                {
                    var pth = Path.Combine(pathToBinaries.FullName, rel);

                    var inf = new FileInfo(pth);
                    if (!inf.Directory.Exists)
                        inf.Directory.Create();
                    if (inf.Exists)
                        inf.Delete();

                    File.Copy(rel, pth);
                }
            }

            //Copy Libraries
            if (Directory.Exists(LibrariesFolder))
            {
                foreach (var item in Directory.GetFiles(LibrariesFolder))
                {
                    if (item == Path.Combine(LibrariesFolder, ".DS_Store")) continue;
                    var target = Path.Combine(pathToBinaries.FullName, item);

                    if (File.Exists(target)) File.Delete(target);
                    File.Copy(item, target);
                }
            }
        }
    }
}
