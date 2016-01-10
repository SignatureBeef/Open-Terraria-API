using System;
using System.IO;

namespace OTA
{
    public static class SafeConsole
    {
        public static TextWriter Error
        {
            get { return Console.Error; }
        }

        public static void WriteLine(string fmt, params object[] args)
        {
            if (!Environment.UserInteractive)
                Console.WriteLine(fmt, args);
        }
    }
}
