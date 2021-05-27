using System;
using System.Reflection;

namespace OTAPI.Patcher.Targets
{
    public static class Common
    {
        public static string GetVersion()
        {
            return typeof(Common).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }

        public static void Log(string message)
        {
            Console.WriteLine($"[ModFw] {message}");
        }

        public static string GetCliValue(string key)
        {
            string find = $"-{key}=";
            var match = Array.Find(Environment.GetCommandLineArgs(), x => x.StartsWith(find, StringComparison.CurrentCultureIgnoreCase));
            return match?.Substring(find.Length)?.ToLower();
        }
    }
}
