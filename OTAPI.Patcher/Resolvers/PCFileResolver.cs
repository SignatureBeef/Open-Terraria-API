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

namespace OTAPI.Patcher.Resolvers;

[MonoMod.MonoModIgnore]
public class PCFileResolver : IFileResolver
{
    public const String TerrariaWebsite = "https://terraria.org";

    public virtual string SupportedDownloadUrl { get; } = $"{TerrariaWebsite}/api/download/pc-dedicated-server/terraria-server-1444.zip";

    public virtual string GetUrlFromHttpResponse(string content)
    {
        var items = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(content);
        var server = items.First(i => i.Contains("terraria-server-", StringComparison.OrdinalIgnoreCase));
        return $"{TerrariaWebsite}/api/download/pc-dedicated-server/{server}";
    }

    public virtual string AquireLatestBinaryUrl()
    {
        Console.WriteLine("Determining the latest TerrariaServer.exe...");
        using var client = new HttpClient();

        var data = client.GetByteArrayAsync($"{TerrariaWebsite}/api/get/dedicated-servers-names").Result;
        var json = System.Text.Encoding.UTF8.GetString(data);

        return GetUrlFromHttpResponse(json);
    }

    public virtual string DetermineInputAssembly(string extractedFolder)
    {
        return Directory.EnumerateFiles(extractedFolder, "TerrariaServer.exe", SearchOption.AllDirectories).Single(x =>
            Path.GetFileName(Path.GetDirectoryName(x)).Equals("Windows", StringComparison.CurrentCultureIgnoreCase)
        );
    }
}
