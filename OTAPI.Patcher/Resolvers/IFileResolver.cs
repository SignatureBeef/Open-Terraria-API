namespace OTAPI.Patcher.Resolvers;

[MonoMod.MonoModIgnore]
public interface IFileResolver
{
    string SupportedDownloadUrl { get; }
    string AquireLatestBinaryUrl();
    string DetermineInputAssembly(string extractedFolder);
}
