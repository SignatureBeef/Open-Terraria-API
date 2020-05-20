using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;

namespace OTAPI.Common
{
    public class Remote
    {
        const String TerrariaWebsite = "https://terraria.org";

        static void Log(string message)
        {
            Console.WriteLine($"[OTAPI] [Setup] {message}");
        }

        static string GetZipUrl()
        {
            Log($"Determining the latest TerrariaServer.exe...");
            using (var client = new HttpClient())
            {
                var data = client.GetByteArrayAsync(TerrariaWebsite).Result;
                var html = System.Text.Encoding.UTF8.GetString(data);

                const String Lookup = ">PC Dedicated Server";

                var offset = html.IndexOf(Lookup, StringComparison.CurrentCultureIgnoreCase);
                if (offset == -1) throw new NotSupportedException();

                var attr_character = html[offset - 1];

                var url = html.Substring(0, offset - 1);
                var url_begin_offset = url.LastIndexOf(attr_character);
                if (url_begin_offset == -1) throw new NotSupportedException();

                url = url.Remove(0, url_begin_offset + 1);

                return TerrariaWebsite + url;
            }
        }

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

            if (!info.Exists || info.GetDirectories().Length == 0)
                ZipFile.ExtractToDirectory(zipPath, directory);

            return directory;
        }

        public static string DownloadServer()
        {
            var zipUrl = GetZipUrl();
            var zipPath = DownloadZip(zipUrl);
            var extracted = ExtractZip(zipPath);

            return Path.Combine(extracted, "1402", "Windows", "TerrariaServer.exe");
        }
    }
}
