// Copyright (C) 2020-2021 DeathCradle
//
// This file is part of Open Terraria API v3 (OTAPI)
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
//using ICSharpCode.SharpZipLib.BZip2;
//using ICSharpCode.SharpZipLib.Tar;

namespace OTAPI.Client.Host
{
    partial class Program
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetDllDirectory(string lpPathName);

        public static void Main(string[] args)
        {
            Console.WriteLine("[OTAPI.Client] Starting!");
            //Environment.SetEnvironmentVariable("DYLD_LIBRARY_PATH", Path.Combine(Environment.CurrentDirectory, "fnalibs", "osx"));

            // https://github.com/FNA-XNA/FNA/wiki/4:-FNA-and-Windows-API#64-bit-support
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetDllDirectory(Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    Environment.Is64BitProcess ? "x64" : "x86"
                ));
            }

            // https://github.com/FNA-XNA/FNA/wiki/7:-FNA-Environment-Variables#fna_graphics_enable_highdpi
            // NOTE: from documentation: 
            //       Lastly, when packaging for macOS, be sure this is in your app bundle's Info.plist:
            //           <key>NSHighResolutionCapable</key>
            //           <string>True</string>
            Environment.SetEnvironmentVariable("FNA_GRAPHICS_ENABLE_HIGHDPI", "1");

            //var osx = Path.Combine(Environment.CurrentDirectory, "osx");
            //Environment.SetEnvironmentVariable("DYLD_LIBRARY_PATH", osx);

            NativeLibrary.SetDllImportResolver(typeof(Microsoft.Xna.Framework.Game).Assembly, (libraryName, assembly, searchPath) =>
            {
                if (_nativeCache.TryGetValue(libraryName, out IntPtr cached))
                    return cached;

                Console.WriteLine("Looking for " + libraryName);

                IEnumerable<string> matches = Enumerable.Empty<string>();

                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    var osx = Path.Combine(Environment.CurrentDirectory, "osx");
                    matches = Directory.GetFiles(osx, "*" + libraryName + "*");
                }

                if (matches.Count() == 1)
                {
                    var match = matches.Single();
                    var handle = NativeLibrary.Load(match);
                    _nativeCache.Add(libraryName, handle);
                    return handle;
                }

                return IntPtr.Zero;
            });

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            AppDomain.CurrentDomain.TypeResolve += CurrentDomain_TypeResolve;

            //InstallLibs();

            // moved to its own class to ensure variables from terraria dont end up throwing missing field exceptions
            IsolatedLaunch.Launch(args);
        }

        //static void InstallLibs()
        //{
        //    // http://fna.flibitijibibo.com/archive/fnalibs.tar.bz2
        //    var zipPath = DownloadZip("http://fna.flibitijibibo.com/archive/fnalibs.tar.bz2");
        //    var extr = ExtractZip(zipPath);

        //    //var osx = Path.Combine(extr, "osx");
        //    //foreach (var item in Directory.GetFiles(osx, "*"))
        //    //{
        //    //    var src = item;
        //    //    var dst = Path.Combine(Environment.CurrentDirectory, Path.GetFileName(item));

        //    //    if (File.Exists(dst)) File.Delete(dst);
        //    //    File.Copy(src, dst);
        //    //}
        //}

        //public static string DownloadZip(string url)
        //{
        //    Console.WriteLine($"[OTAPI.Client] Downloading {url}");
        //    var uri = new Uri(url);
        //    string filename = Path.GetFileName(uri.AbsolutePath);
        //    if (!String.IsNullOrWhiteSpace(filename))
        //    {
        //        var savePath = Path.Combine(Environment.CurrentDirectory, filename);

        //        if (!File.Exists(savePath))
        //        {
        //            new System.Net.WebClient().DownloadFile(url, savePath);
        //            //using var client = new HttpClient();
        //            //var data = client.GetByteArrayAsync(url).Result;
        //            //File.WriteAllBytes(savePath, data);
        //        }

        //        return savePath;
        //    }
        //    else throw new NotSupportedException();
        //}

        //public static string ExtractZip(string zipPath)
        //{
        //    //var directory = Path.GetFileNameWithoutExtension(zipPath);
        //    //var info = new DirectoryInfo(directory);
        //    //Console.WriteLine($"[OTAPI.Client] Extracting to {directory}");

        //    //if (info.Exists) info.Delete(true);

        //    //info.Refresh();

        //    //if (!info.Exists || info.GetDirectories().Length == 0)
        //    //{

        //    var newname = Path.GetFileNameWithoutExtension(zipPath);

        //    using var raw = File.OpenRead(zipPath);
        //    using var ms = new MemoryStream();
        //    BZip2.Decompress(raw, ms, false);
        //    ms.Seek(0, SeekOrigin.Begin);
        //    newname = Path.GetFileNameWithoutExtension(newname);

        //    using var tarArchive = TarArchive.CreateInputTarArchive(ms, System.Text.Encoding.UTF8);


        //    if (Directory.Exists(newname))
        //        Directory.Delete(newname, true);

        //    Directory.CreateDirectory(newname);

        //    var abs = Path.GetFullPath(newname);
        //    tarArchive.ExtractContents(abs);
        //    tarArchive.Close();


        //    return newname;
        //}

        private static Assembly CurrentDomain_TypeResolve(object sender, ResolveEventArgs args)
        {
            Console.WriteLine("Looking for type: " + args.Name);
            return null;
        }

        static Dictionary<string, IntPtr> _nativeCache = new Dictionary<string, IntPtr>();
        static Dictionary<string, Assembly> _assemblyCache = new Dictionary<string, Assembly>();

        static void CacheAssembly(Assembly assembly)
        {
            var name = assembly.GetName().Name;
            if (!_assemblyCache.ContainsKey(name))
            {
                _assemblyCache.Add(name, assembly);
            }
        }

        static Assembly LoadAndCacheAssembly(string filePath)
        {
            Assembly result = null;

            //var match = _assemblyCache.FirstOrDefault(f => f.FilePath == filePath);
            //if (match.Assembly != null)
            //    result = match.Assembly;
            //else
            //{
            var abs = Path.Combine(Environment.CurrentDirectory, filePath);
            result = Assembly.LoadFile(abs);
            CacheAssembly(result);
            //    _assemblyCache.Add((result, filePath));
            //}

            return result;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var asmName = new AssemblyName(args.Name);

            if (_assemblyCache.TryGetValue(asmName.Name, out Assembly cached))
                return cached;

            Console.WriteLine("Looking for: " + args.Name);
            if (args.Name.StartsWith("Terraria") || args.Name.StartsWith("OTAPI")
                || args.Name.StartsWith("ReLogic") // TODO remove this from the embedded resouces, then it wont get here unless bad IL.
                )
            {
                return typeof(Terraria.Program).Assembly;
                //return LoadAndCacheAssemlbly(System.IO.Path.Combine(Environment.CurrentDirectory, "OTAPI.exe"));
            }
            else if (args.Name.StartsWith("ImGuiNET"))
            {
                return LoadAndCacheAssembly(Path.Combine(Environment.CurrentDirectory, "ImGui.NET.dll"));
            }
            else
            {
                //var matches = System.IO.Directory.GetFiles("../../../../OTAPI.Patcher/bin/Debug/net5.0/EmbeddedResources", "*.dll");

                //var namedMatch = matches.FirstOrDefault(path => System.IO.Path.GetFileNameWithoutExtension(path) == args.Name);
                //if (namedMatch != null)
                //    return LoadAndCacheAssemlbly(namedMatch);


                //var asd = "";
                //foreach (var file in matches)
                //{
                //    var assembly = LoadAndCacheAssemlbly(file);
                //    if (args.Name == assembly.GetName().Name)
                //        return assembly;
                //}

                var root = typeof(Terraria.Program).Assembly;
                string resourceName = asmName.Name + ".dll";

                if (File.Exists(resourceName))
                    return LoadAndCacheAssembly(Path.Combine(Environment.CurrentDirectory, resourceName));

                Console.WriteLine("Looking for res: " + resourceName);
                //Console.WriteLine("Looking in: " + String.Join(",", root.GetManifestResourceNames()));
                string text = Array.Find(root.GetManifestResourceNames(), (string element) => element.EndsWith(resourceName));
                if (text != null)
                {
                    Console.WriteLine("Loading from resources " + resourceName);
                    using var stream = root.GetManifestResourceStream(text);
                    byte[] array = new byte[stream.Length];
                    stream.Read(array, 0, array.Length);
                    stream.Seek(0, SeekOrigin.Begin);

                    try
                    {
                        var asm = System.Reflection.Assembly.Load(array);
                        return asm;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

            }
            return null;
        }
    }
}
