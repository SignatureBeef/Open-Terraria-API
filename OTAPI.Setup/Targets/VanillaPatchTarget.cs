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
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using ModFramework.Targets;

namespace OTAPI.Setup.Targets
{
    [MonoMod.MonoModIgnore]
    public class VanillaPatchTarget : IPatchTarget
    {
        const String TerrariaWebsite = "https://terraria.org";

        public string AquireLatestBinaryUrl()
        {
            this.Log("Determining the latest TerrariaServer.exe...");
            using var client = new HttpClient();

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

        public string DetermineInputAssembly(string extractedFolder)
        {
            return Directory.EnumerateFiles(extractedFolder, "TerrariaServer.exe", SearchOption.AllDirectories).Single(x =>
                Path.GetFileName(Path.GetDirectoryName(x)).Equals("Windows", StringComparison.CurrentCultureIgnoreCase)
            );
        }

        public string GetZipUrl()
        {
            var cli = this.GetCliValue("latestVanilla");

            if (cli != "n")
            {
                int attempts = 5;
                do
                {
                    Console.Write("Download the latest binary? y/[N]: ");

                    var input = Console.ReadLine().ToLower();

                    if (input.Equals("y", StringComparison.CurrentCultureIgnoreCase))
                        return AquireLatestBinaryUrl();
                    else if (input.Equals("n", StringComparison.CurrentCultureIgnoreCase))
                        break;
                } while (attempts-- > 0);
            }

            return "https://terraria.org/system/dedicated_servers/archives/000/000/044/original/terraria-server-1421.zip?1617223487";
        }
    }
}
