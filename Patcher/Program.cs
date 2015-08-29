using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

namespace OTA.Patcher
{
    public enum PatchMode
    {
        Server,
        Client
    }

    public class Program
    {
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

            OTAPatcher.DefaultProcess(args);
        }
    }

    public static class OTAPatcher
    {
        public const String OTAGuid = "9f7bca2e-4d2e-4244-aaae-fa56ca7797ec";
        public const Int32 Build = 5;

        public static void Copy(DirectoryInfo root, string project, string to, string pluginName = null, bool debugFolder = true)
        {
            var projectBinary = (pluginName ?? project).Replace("-", ".");
            var p = debugFolder ? Path.Combine(root.FullName, project, "bin", "x86", "Debug") : Path.Combine(root.FullName, project);
            if (!Directory.Exists(p))
                p = debugFolder ? Path.Combine(root.FullName, project, "bin", "Debug") : Path.Combine(root.FullName, project);

            //From the project
            var dllFrom = Path.Combine(p, projectBinary + ".dll");
            var ddbFrom = Path.Combine(p, projectBinary + ".dll.mdb");
            var pdbFrom = Path.Combine(p, projectBinary + ".pdb");

            //To the patcher
            var dllTo = Path.Combine(to, projectBinary + ".dll");
            var ddbTo = Path.Combine(to, projectBinary + ".dll.mdb");
            var pdbTo = Path.Combine(to, projectBinary + ".pdb");

            CopyDep(dllFrom, dllTo);
            CopyDep(ddbFrom, ddbTo);
            CopyDep(pdbFrom, pdbTo);
        }

        public static void EnsurePath(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }
        }

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

        public class InjectorEventArgs
        {
            public Injector Injector { get; internal set; }
        }

        public static event EventHandler<InjectorEventArgs> PerformPatch;

        public class CopyDependenciesEventArgs
        {
            public DirectoryInfo RootDirectory { get; internal set; }
        }

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
                    Copy(root, "API", Environment.CurrentDirectory, "OTA", true);

                    if (!String.IsNullOrEmpty(OTAProjectDirectory))
                    {
                        Copy(new DirectoryInfo(Path.Combine(root.FullName, OTAProjectDirectory)), "API", Environment.CurrentDirectory, "OTA", true);
                    }
                    //            Copy(root, "TDSM-Core", Path.Combine(Environment.CurrentDirectory, "Plugins"));
                    //            Copy(root, "Binaries", Path.Combine(Environment.CurrentDirectory), "TDSM.API");
                    //Copy (root, "Restrict", Path.Combine (Environment.CurrentDirectory, "Plugins"), "RestrictPlugin");
                    Copy(root, "External", Path.Combine(Environment.CurrentDirectory, "Libraries"), "KopiLua", false);
                    Copy(root, "External", Path.Combine(Environment.CurrentDirectory, "Libraries"), "NLua", false);
                    Copy(root, "External", Path.Combine(Environment.CurrentDirectory, "Libraries"), "ICSharpCode.SharpZipLib", false);
                    Copy(root, "External", Path.Combine(Environment.CurrentDirectory, "Libraries"), "Mono.Nat", false);
                    //            Copy(root, "tdsm-core", Path.Combine(Environment.CurrentDirectory, "Libraries"), "Newtonsoft.Json", true);
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
                    Copy(root, "API", Environment.CurrentDirectory, "OTA", true);

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
                patcher.SkipMenu();
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
                Console.Write("Ok\nEnabling rain...");
                patcher.EnableRaining();

                Console.Write("Ok\nFixing world removal...");
                patcher.PathFileIO();
                Console.Write("Ok\nRouting network message validity...");
                patcher.HookValidPacketState();

                Console.Write("Ok\nRemoving port forwarding functionality...");
                patcher.FixNetplay();
                Console.Write("Ok\nFixing NPC AI crashes...");
                patcher.FixRandomErrors();
                //            patcher.DetectMissingXNA();

                Console.Write("Ok\n");
                patcher.InjectHooks();

                Console.Write("Ok\nUpdating to .NET v4.5.1...");
                patcher.SwitchFramework("4.5.1");
                Console.Write("Ok\nPatching Newtonsoft.Json...");
                patcher.PatchJSON();

                //            patcher.SwapToVanillaTile(); //Holy shit batman! it works

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

                Console.Write("Ok\nHooking start...");
                patcher.HookProgramStart(PatchMode);
                Console.Write("Ok\n");

                if (PerformPatch != null)
                    PerformPatch.Invoke(null, new InjectorEventArgs()
                        {
                            Injector = patcher
                        });
                
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("Saving to {0}...", output);
            patcher.Save(PatchMode, output, Build, OTAGuid, fileName);
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

        public static void PromptUserToRun(string output, bool isMono)
        {
            Console.WriteLine("Press [y] to run {0}, any other key will exit . . .", output);
            if (Console.ReadKey(true).Key == ConsoleKey.Y)
            {
                Run(output, isMono);
            }
        }

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
                    var asm = System.Reflection.Assembly.Load(ms.ToArray());
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
                                asm.EntryPoint.Invoke(null, null);

                        }
                        else if (PatchMode == PatchMode.Client)
                        {
                            asm.EntryPoint.Invoke(null, null);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }

        public class ConfigModification
        {
            public string OfficialLinePrefix { get; set; }

            public int Offset { get; set; }

            public string[] Modifications { get; set; }
        }

        public  static string PatchConfig(string[] input, string targetFile = "serverconfig.mods.json")
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

        public static DirectoryInfo GetBinariesFolder()
        {
            var pathToBinaries = new DirectoryInfo(Environment.CurrentDirectory);
            while (!Directory.Exists(Path.Combine(pathToBinaries.FullName, "Binaries")))
            {
                pathToBinaries = pathToBinaries.Parent;
            }
            return new DirectoryInfo(Path.Combine(pathToBinaries.FullName, "Binaries"));
        }

        public  static void GenerateConfig(string official = "serverconfig.txt", string additional = "additional.config", string output = "server.config")
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

        public static List<String> BinariesFiles = new List<String>(new string[]
            {
                "OTA.dll",
                "OTA.dll.mdb",
                "OTA.pdb",
                "Libraries" + Path.DirectorySeparatorChar + "Newtonsoft.Json.dll",
                "Libraries" + Path.DirectorySeparatorChar + "Newtonsoft.Json.pdb",
                "Libraries" + Path.DirectorySeparatorChar + "NLua.dll",
                "Patcher.exe",
                "Patcher.pdb",
                //                "Vestris.ResourceLib.dll",
                "Libraries" + Path.DirectorySeparatorChar + "KopiLua.dll",
                "Libraries" + Path.DirectorySeparatorChar + "ICSharpCode.SharpZipLib.dll",
                "Libraries" + Path.DirectorySeparatorChar + "Mono.Nat.dll",
                "Libraries" + Path.DirectorySeparatorChar + "Mono.Nat.pdb",

                "start-server.bat",
                "start-server.sh",
                "start-server.cmd"
            });

        public static void UpdateBinaries()
        {
            var pathToBinaries = GetBinariesFolder();
            if (!pathToBinaries.Exists)
            {
                Console.WriteLine("Failed to copy to binaries.");
                return;
            }

            BinariesFiles.Add(OutputName + ".exe");

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
            foreach (var item in Directory.GetFiles("Libraries"))
            {
                if (item == "Libraries/.DS_Store") continue;
                var target = Path.Combine(pathToBinaries.FullName, item);

                if (File.Exists(target)) File.Delete(target);
                File.Copy(item, target);
            }
        }
    }
}
