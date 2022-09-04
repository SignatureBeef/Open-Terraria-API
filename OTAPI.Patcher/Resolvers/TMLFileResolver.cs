using System.IO;
using System.Linq;

namespace OTAPI.Patcher.Resolvers;

[MonoMod.MonoModIgnore]
public class TMLFileResolver : IFileResolver
{
    public virtual string SupportedDownloadUrl { get; } = "https://github.com/tModLoader/tModLoader/releases/download/v2022.07.58.3/tModLoader.zip";

    public virtual string AquireLatestBinaryUrl() => SupportedDownloadUrl;

    public virtual string DetermineInputAssembly(string extractedFolder)
            => Directory.EnumerateFiles(extractedFolder, "tModLoader.dll", SearchOption.TopDirectoryOnly).Single();
}