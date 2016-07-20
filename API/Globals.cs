using System;
using System.IO;
using System.Reflection;

namespace OTA
{
    /// <summary>
    /// Release phase.
    /// </summary>
    public enum ReleasePhase : ushort
    {
        //p
        PreAlpha = 0x70,
        //a
        Alpha = 0x61,
        //b
        Beta = 0x62,
        //rc
        ReleaseCandiate = 0x72 | (0x63 << 8),
        //lr
        LiveRelease = 0x6C | (0x72 << 8)
    }

    /// <summary>
    /// Globals for OTA.
    /// </summary>
    public static class Globals
    {
        public static class Version
        {
            public const Int32 Major = 1;
            public const Int32 Minor = 0;
            public const ReleasePhase Phase = ReleasePhase.Beta;
        }

        public static string BuildInfo
        {
            get { return String.Format("{0}.{1}{2}", Version.Major, Version.Minor, PhaseToSuffix(Version.Phase)); }
        }

        public const Int32 TerrariaRelease = 172;
        public const String TerrariaVersion = "1.3.2";

        private const String WorldDirectory = "Worlds";
        private const String PluginDirectory = "Plugins";
        private const String DataDirectory = "Data";
        private const String LibrariesDirectory = "Libraries";
        private const String CharacterData = "Characters";
        private const String LogFolder = "Logs";

        public static volatile bool Exit = false;

        /// <summary>
        /// Gets or sets the current state of the server.
        /// </summary>
        public static ServerState CurrentState { get; set; }

        /// <summary>
        /// The current directory
        /// </summary>
        /// TODO See if this should be renamed
        public static string SavePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        //public static bool IsPatching { get; set; }

#if Full_API
        public const bool FullAPIDefined = true;
#else
        public const bool FullAPIDefined = false;
#endif

        /// <summary>
        /// Gets the world save path.
        /// </summary>
        /// <value>The world path.</value>
        public static string WorldPath { get; } = Path.Combine(SavePath, WorldDirectory);

        /// <summary>
        /// Gets the plugin folder.
        /// </summary>
        /// <value>The plugin path.</value>
        public static string PluginPath { get; } = Path.Combine(SavePath, PluginDirectory);

        /// <summary>
        /// Gets the libraries folder.
        /// </summary>
        /// <value>The libraries path.</value>
        public static string LibrariesPath { get; } = Path.Combine(SavePath, LibrariesDirectory);

        /// <summary>
        /// Gets the data folder.
        /// </summary>
        /// <value>The data path.</value>
        public static string DataPath { get; } = Path.Combine(SavePath, DataDirectory);

        /// <summary>
        /// Gets the character data folder.
        /// </summary>
        /// <value>The character data path.</value>
        public static string CharacterDataPath { get; } = Path.Combine(SavePath, DataDirectory, CharacterData);


        /// <summary>
        /// Gets the log folder.
        /// </summary>
        /// <value>The log folder.</value>
        public static string LogFolderPath { get; } = Path.Combine(Globals.DataPath, LogFolder);

        /// <summary>
        /// Creates default required folders
        /// </summary>
        public static void Touch()
        {
            if (!Directory.Exists(SavePath)) Directory.CreateDirectory(SavePath);
            if (!Directory.Exists(PluginPath)) Directory.CreateDirectory(PluginPath);
            if (!Directory.Exists(LibrariesPath)) Directory.CreateDirectory(LibrariesPath);
            if (!Directory.Exists(DataPath)) Directory.CreateDirectory(DataPath);
            if (!Directory.Exists(CharacterDataPath)) Directory.CreateDirectory(CharacterDataPath);
        }

        /// <summary>
        /// Turns a phase into its textual representation
        /// </summary>
        /// <param name="phase"></param>
        /// <returns></returns>
        public static string PhaseToSuffix(ReleasePhase phase)
        {
            var suffix = "";

            var value = (ushort)phase;
            byte chr = 0;
            for (var i = 0; i < 16; i++)
            {
                if ((value & (1 << i)) != 0) chr |= (byte)(1 << (i % 8));

                if (i > 0 && (i + 1) % 8 == 0)
                {
                    if (chr > 0) suffix += (char)chr;
                    chr = 0;
                }
            }

            return suffix;
        }
    }
}
