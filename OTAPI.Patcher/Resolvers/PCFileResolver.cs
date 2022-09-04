using System;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace OTAPI.Patcher.Resolvers;

[MonoMod.MonoModIgnore]
public class PCFileResolver : IFileResolver
{
    public const String TerrariaWebsite = "https://terraria.org";

    public virtual string SupportedDownloadUrl { get; } = $"{TerrariaWebsite}/api/download/pc-dedicated-server/terraria-server-1436.zip";

    public virtual string GetUrlFromHttpResponse(string content)
    {
        var items = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(content);
        var server = items.Single(i => i.Contains("terraria-server-", StringComparison.OrdinalIgnoreCase));
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