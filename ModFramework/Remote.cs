/*
Copyright (C) 2020 DeathCradle

This file is part of Open Terraria API v3 (OTAPI)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/
using ModFramework.Targets;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;

namespace ModFramework
{
    [MonoMod.MonoModIgnore]
    public static class Remote
    {
        static void Log(string message)
        {
            Console.WriteLine($"[ModFw] [Setup] {message}");
        }
        public static void Log(this IPatchTarget target, string message) => Log(message);

        static string DownloadZip(string url)
        {
            Log($"Downloading {url}");
            var uri = new Uri(url);
            string filename = Path.GetFileName(uri.AbsolutePath);
            if (!String.IsNullOrWhiteSpace(filename))
            {
                var savePath = Path.Combine(Environment.CurrentDirectory, filename);

                if (!File.Exists(savePath))
                {
                    using (var client = new HttpClient())
                    {
                        var data = client.GetByteArrayAsync(url).Result;
                        File.WriteAllBytes(savePath, data);
                    }
                }

                return savePath;
            }
            else throw new NotSupportedException();
        }

        static string ExtractZip(string zipPath)
        {
            var directory = Path.GetFileNameWithoutExtension(zipPath);
            var info = new DirectoryInfo(directory);
            Log($"Extracting to {directory}");

            if (info.Exists) info.Delete(true);

            info.Refresh();

            if (!info.Exists || info.GetDirectories().Length == 0)
                ZipFile.ExtractToDirectory(zipPath, directory);

            return directory;
        }

        public static string GetCliValue(string key)
        {
            string find = $"-{key}=";
            var match = Environment.GetCommandLineArgs().FirstOrDefault(x => x.StartsWith(find, StringComparison.CurrentCultureIgnoreCase));
            return match?.Substring(find.Length)?.ToLower();
        }

        static IPatchTarget DeterminePatchTarget()
        {
            var cli = GetCliValue("patchTarget");
            if (cli == "m") return new TMLPatchTarget();
            if (cli == "v") return new VanillaPatchTarget();

            int attempts = 5;
            do
            {
                Console.Write("What (v)anilla or t(m)odloader? m/[V]: ");

                var input = Console.ReadLine().ToLower();

                if (input.Equals("m", StringComparison.CurrentCultureIgnoreCase))
                    return new TMLPatchTarget();

                if (input.Equals("v", StringComparison.CurrentCultureIgnoreCase))
                    break;
            } while (attempts-- > 0);

            return new VanillaPatchTarget();
        }

        public static string DownloadServer()
        {
            var target = DeterminePatchTarget();
            var zipUrl = target.GetZipUrl();
            var zipPath = DownloadZip(zipUrl);
            var extracted = ExtractZip(zipPath);

            return target.DetermineInputAssembly(extracted);
        }
    }
}
