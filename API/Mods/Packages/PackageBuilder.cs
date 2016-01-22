using Mono.Cecil;
using OTA.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace OTA.Mod.Packages
{
    public class PackageBuilder
    {
        private const int PackageVersion = 1;

        public string PackageDirectory { get; set; }
        public string FileName { get; set; }

        public string Name { get; set; }

        public PackageBuilder() { }

        public PackageBuilder(string name)
        {
            this.Name = name;
        }

        public static string WorkingDirectory
        {
            get { return Path.Combine(Globals.DataPath, "Mod"); }
        }

        private static void CheckWorkingDirectory()
        {
            if (!Directory.Exists(WorkingDirectory))
                Directory.CreateDirectory(WorkingDirectory);
        }

        public static PackageBuilder CreateFromDirectory(string name, string directory)
        {
            var pkg = new PackageBuilder(name)
            {
                PackageDirectory = directory
            };
            pkg.FileName = pkg.Compile();
            return pkg;
        }

        public static PackageBuilder LoadFromFile(string filename)
        {
            var pkg = new PackageBuilder()
            {
                FileName = filename
            };
            pkg.Build();
            return pkg;
        }

        protected virtual string Compile()
        {
            CheckWorkingDirectory();
            var filename = Path.Combine(WorkingDirectory, this.Name + ".zip");

            if (File.Exists(filename)) File.Delete(filename);

            using (var sw = new BinaryWriter(File.OpenWrite(filename)))
            {
                sw.Write(PackageVersion);
                sw.Flush();

                using (var zs = new System.IO.Compression.ZipArchive(sw.BaseStream, System.IO.Compression.ZipArchiveMode.Create, true))
                {
                    var directoryInfo = new DirectoryInfo(this.PackageDirectory);

                    foreach (var kind in new[] { "*.vb", "*.cs", "*.dll", "*.png", "*.xnb" })
                        foreach (var current in directoryInfo.EnumerateFileSystemInfos(kind, SearchOption.AllDirectories))
                        {
                            if (current is FileInfo)
                            {
                                var entryName = current.FullName
                                    .Remove(0, Path.Combine(System.Environment.CurrentDirectory, this.PackageDirectory).Length)
                                    .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                                zs.CreateEntryFromFile(current.FullName, entryName);
                            }
                        }
                }
            }

            return filename;
        }

        //private AssemblyDefinition LoadCecilAssembly(FileInfo file)
        //{
        //    //Load the Terraria assembly
        //    using (var ms = new MemoryStream())
        //    {
        //        using (var fs = file.OpenRead())
        //        {
        //            var buff = new byte[256];
        //            while (fs.Position < fs.Length)
        //            {
        //                var task = fs.Read(buff, 0, buff.Length);
        //                ms.Write(buff, 0, task);
        //            }
        //        }

        //        ms.Seek(0L, SeekOrigin.Begin);
        //        return AssemblyDefinition.ReadAssembly(ms);
        //    }
        //}

        //private void LoadPlatformPlugin(FileInfo file)
        //{
        //    var asm = LoadCecilAssembly(file);

        //    var isWindows = true;

        //    var msXnaToken = new byte[]
        //    {
        //        132,44,248,190,29,229,5,83
        //    };

        //    //Patch the assembly version differences in Terraria's different platform builds.
        //    var currentVersion = typeof(PackageBuilder).Assembly.GetReferencedAssemblies().Single(x => x.Name == "Terraria").Version;

        //    //Check references
        //    bool addXna = false;
        //    foreach (var reference in asm.MainModule.AssemblyReferences)
        //    {
        //        if (isWindows)
        //        {
        //            //Replace FNA with Microsfot.Xna.Framework.Game
        //            //Add Microsoft.Xna.Framework.Graphics
        //            if (reference.Name == "FNA")
        //            {
        //                reference.Name = "Microsoft.Xna.Framework";
        //                reference.PublicKey = null;
        //                reference.PublicKeyToken = msXnaToken;
        //                reference.Version = new Version(4, 0, 0, 0);

        //                addXna = true;
        //            }
        //        }
        //        else
        //        {
        //            //Replace Microsft.Xna.* with FNA
        //            if (reference.Name.StartsWith("Microsoft.Xna"))
        //            {
        //                reference.Name = "FNA";
        //                reference.PublicKey = null;
        //                reference.PublicKeyToken = null;
        //                reference.Version = new Version(0, 0, 0, 1);
        //            }
        //        }

        //        if (reference.Name == "Terraria")
        //        {
        //            reference.Version = currentVersion;
        //        }
        //    }

        //    // Microsoft.Xna.Framework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553
        //    // Microsoft.Xna.Framework.Game, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553
        //    // Microsoft.Xna.Framework.Graphics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553
        //    // Microsoft.Xna.Framework.Xact, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553

        //    if (addXna)
        //    {
        //        foreach (var name in new[] { "Game", "Graphics", "Xact" })
        //        {
        //            var reference = new AssemblyNameReference("Microsoft.Xna.Framework." + name, new Version(4, 0, 0, 0));
        //            reference.PublicKey = null;
        //            reference.PublicKeyToken = msXnaToken;
        //            asm.MainModule.AssemblyReferences.Add(reference);
        //        }
        //    }

        //    asm.Write("out.dll");

        //    var plugin = Plugin.PluginManager.LoadPluginFromPath("out.dll");
        //    if (plugin != null)
        //    {
        //        ProgramLog.Log("LOADED PLUGIN!");
        //        Plugin.PluginManager.RegisterPlugin(plugin);
        //        ProgramLog.Log("Registered!");
        //        //if (plugin.InitializeAndHookUp())
        //        //{
        //        //    //_plugins.Add(plugin.Name.ToLower().Trim(), plugin);

        //        //    if (plugin.EnableEarly)
        //        //        plugin.Enable();

        //        //    //LoadScheduled(true);
        //        //}
        //    }
        //    else
        //    {
        //        ProgramLog.Log("FAILED!");
        //    }
        //}

        protected virtual void Build()
        {
            //var files = new List<String>();
            //var directoryInfo = new DirectoryInfo(this.Directory);

            //foreach (var kind in new[] { "*.dll" })
            //    foreach (var current in directoryInfo.EnumerateFileSystemInfos(kind, SearchOption.AllDirectories))
            //    {
            //        if (current is FileInfo)
            //        {
            //            var fi = current as FileInfo;
            //            LoadPlatformPlugin(fi);
            //        }
            //    }

        }

        protected virtual void Extract()
        {
            CheckWorkingDirectory();
            //Extract file to this.Folder (generate if null)

            if (String.IsNullOrEmpty(this.PackageDirectory))
            {
                this.PackageDirectory = Path.Combine(WorkingDirectory, this.Name);
            }

            if (System.IO.Directory.Exists(this.PackageDirectory))
                System.IO.Directory.Delete(this.PackageDirectory, true);

            using (var sr = new BinaryReader(File.OpenRead(this.FileName)))
            {
                var version = sr.BaseStream.ReadByte();

                if (version == 1)
                {

                }

                using (var zs = new System.IO.Compression.ZipArchive(sr.BaseStream, ZipArchiveMode.Read))
                {
                    zs.ExtractToDirectory(this.PackageDirectory);
                }
            }
        }

        public void Run()
        {
            Extract();
            Build();

            var xna = Misc.Platform.Type == Misc.Platform.PlatformType.WINDOWS;
            var path = Path.Combine(this.PackageDirectory, xna ? "XNA" : "FNA");

            var directoryInfo = new DirectoryInfo(path);

            foreach (var kind in new[] { "*.dll" })
                foreach (var current in directoryInfo.EnumerateFileSystemInfos(kind, SearchOption.AllDirectories))
                {
                    if (current is FileInfo)
                    {
                        var fi = current as FileInfo;

                        var plugin = Plugin.PluginManager.LoadPluginFromPath(fi.FullName);
                        if (plugin != null)
                        {
                            Plugin.PluginManager.RegisterPlugin(plugin);
                        }
                        else
                        {
                            ProgramLog.Log("Failed to load mod file: " + fi.Name);
                        }
                    }
                }
        }
    }
}
