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
            Unknown = 0,
            Linux = 1,
            Mac = 2,
            Windows = 3
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
                    ) ? PlatformType.Mac : PlatformType.Linux;
                    break;
                case PlatformID.MacOSX:
                    Type = PlatformType.Mac;
                    break;
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                case PlatformID.Xbox:
                    Type = PlatformType.Windows;
                    break;
                default:
                    Type = PlatformType.Unknown;
                    break;
            }
        }
    }
}