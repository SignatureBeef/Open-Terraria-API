using System;
using System.IO;
using System.Linq;

namespace OTAPI.Common
{
    public class ClientInstallPath<ITarget>
        where ITarget : IInstallDiscoverer
    {
        public ITarget Target { get; set; }
        public string Path { get; set; }
        public string GetResource(string fileName) => Target.GetResource(fileName, Path);
        public string GetResourcePath() => Target.GetResourcePath(Path);
    }

    public class ClientHelpers
    {
        static IInstallDiscoverer[] Discoverers = new IInstallDiscoverer[]
        {
            new MacOSInstallDiscoverer(),
            new WindowsInstallDiscoverer()
        };

        public static ClientInstallPath<IInstallDiscoverer> DetermineClientInstallPath() => DetermineClientInstallPath(Discoverers);

        public static ClientInstallPath<ITarget> DetermineClientInstallPath<ITarget>(ITarget[] discoverers)
            where ITarget : IInstallDiscoverer
        {
            var installPaths = discoverers
                .SelectMany(x => x.FindInstalls().Select(i => new ClientInstallPath<ITarget> { Path = i, Target = x }));

            if (installPaths.Count() == 1)
                return installPaths.Single();
            else if (installPaths.Count() > 1)
            {
                Console.WriteLine("More than one install path found; please specify which: ");

                for (var i = 0; i < installPaths.Count(); i++)
                {
                    Console.WriteLine($"\t{i} - {installPaths.ElementAt(i).Path}");
                }

                Console.Write("Choice: ");
                var key = Console.ReadKey();
                Console.WriteLine();
                if (Int32.TryParse(key.KeyChar.ToString(), out int index))
                {
                    if (index < installPaths.Count() && index >= 0)
                        return installPaths.ElementAt(index);
                    else Console.Error.WriteLine("Invalid option: " + index);
                }
                else Console.Error.WriteLine("Invalid option, expected a number.");
            }

            throw new DirectoryNotFoundException();
        }
    }
}
