using System.IO;

namespace OTAPI.Common;

public static class Utils
{
    public static void CopyFiles(string sourcePath, string targetPath)
    {
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));

        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
    }

    public static void TransferFile(string src, string dst)
    {
        if (!File.Exists(src))
            throw new FileNotFoundException("Source binary not found, was it rebuilt before running the installer?\n" + src);

        if (File.Exists(dst))
            File.Delete(dst);

        File.Copy(src, dst);
    }
}
