using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace OTAPI.Common
{
    public static class IntegrityHashes
    {
        public static IEnumerable<string> WindowsClient = new[] {
            "F3E96856E497906842C7C88C97D320EB4669A199" // 1.4.2.3
            ,"81B3E10FCDCA2535F4F00D53F30F18C4F32ECBC7" // 1.4.3.2
        };
        public static IEnumerable<string> MacOSClient = Enumerable.Empty<string>();
        public static IEnumerable<string> LinuxClient = Enumerable.Empty<string>();

        public static IEnumerable<string> Clients = WindowsClient.Union(MacOSClient).Union(LinuxClient);

        public static string ComputeHash(string path)
        {
            using var fs = File.Open(path, FileMode.Open);
            using var bs = new BufferedStream(fs);
            using var sha1 = new SHA1Managed();

            var hash = sha1.ComputeHash(bs);
            var formatted = new StringBuilder(2 * hash.Length);
            foreach (var b in hash)
                formatted.AppendFormat("{0:X2}", b);

            return formatted.ToString();
        }
    }
}
