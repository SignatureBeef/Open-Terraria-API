using System;
using System.IO;

namespace OTAPI.Common
{
    public class ClientHelpers
    {
        static string[] SearchPaths { get; } = new[]
        {
            "/Users/[USER_NAME]/Library/Application Support/Steam/steamapps/common/Terraria/Terraria.app/Contents/",
            "/Applications/Terraria.app/Contents/",
        };

        public static bool IsValidInstallPath(string folder)
        {
            bool valid = Directory.Exists(folder);

            var macOS = Path.Combine(folder, "MacOS");
            var resources = Path.Combine(folder, "Resources");

            var startScript = Path.Combine(macOS, "Terraria");
            var startBin = Path.Combine(macOS, "Terraria.bin.osx");
            //var assembly = Path.Combine(resources, "Terraria.exe");

            valid &= Directory.Exists(macOS);
            valid &= Directory.Exists(resources);

            valid &= File.Exists(startScript);
            valid &= File.Exists(startBin);
            //valid &= File.Exists(assembly);

            return valid;
        }

        public static string DetermineClientInstallPath()
        {
            foreach (var path in SearchPaths)
            {
                var formatted = path.Replace("[USER_NAME]", Environment.UserName);
                if (IsValidInstallPath(formatted))
                    return formatted;
            }

            int count = 5;
            do
            {
                Console.Write("What is the Terraria client install FOLDER?: ");
                var path = Console.ReadLine();

                if (!String.IsNullOrWhiteSpace(path))
                {
                    if (IsValidInstallPath(path))
                        return path;
                }

                Console.WriteLine("Invalid folder or wrong install folder.");
            }
            while (count-- > 0);

            throw new DirectoryNotFoundException();
        }
    }
}
