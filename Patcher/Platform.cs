using System;
using System.IO;

namespace OTA.Misc
{
    /// <summary>
    /// Platform detections
    /// </summary>
    public static class Platform
    {
        static Platform()
        {
            InitPlatform();
        }

        /// <summary>
        /// Gets the type of platform the application is running on
        /// </summary>
        /// <value>The type.</value>
        public static PlatformType Type { get; set; }

        /// <summary>
        /// Platform type.
        /// </summary>
        public enum PlatformType : int
        {
            UNKNOWN = 0,
            LINUX = 1,
            MAC = 2,
            WINDOWS = 3
        }

        /// <summary>
        /// Determines the current platform
        /// </summary>
        public static void InitPlatform()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    Type = (
                        Directory.Exists("/Applications")
                        && Directory.Exists("/System")
                        && Directory.Exists("/Users")
                        && Directory.Exists("/Volumes")
                    ) ? PlatformType.MAC : PlatformType.LINUX;
                    break;
                case PlatformID.MacOSX:
                    Type = PlatformType.MAC;
                    break;
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                case PlatformID.Xbox:
                    Type = PlatformType.WINDOWS;
                    break;
                default:
                    Type = PlatformType.UNKNOWN;
                    break;
            }
        }
    }
}