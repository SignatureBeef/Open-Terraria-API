using System;
using System.IO;
using System.Linq;

namespace OTAPI.Patcher.Resolvers;

[MonoMod.MonoModIgnore]
public class MobileFileResolver : PCFileResolver
{
    public override string SupportedDownloadUrl => $"{TerrariaWebsite}/api/download/mobile-dedicated-server/MobileTerrariaServer.zip";

    public override string DetermineInputAssembly(string extractedFolder)
    {
        var zip = Directory.EnumerateFiles(extractedFolder, "Windows_MobileServer*.zip", SearchOption.AllDirectories).Single();
        extractedFolder = Common.ExtractZip(zip);

        return Directory.EnumerateFiles(extractedFolder, "TerrariaServer.exe", SearchOption.AllDirectories).Single(x =>
            Path.GetFileName(Path.GetDirectoryName(x)).Equals("WindowsServer", StringComparison.CurrentCultureIgnoreCase)
        );
    }

    public override string GetUrlFromHttpResponse(string content)
    {
        var items = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(content);
        var server = items.Single(i => i.Contains("MobileTerrariaServer", StringComparison.OrdinalIgnoreCase));
        return $"{TerrariaWebsite}/api/download/mobile-dedicated-server/{server}";
    }
}