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
        PreAlpha = 0x70,                        //p
        Alpha = 0x61,                           //a
        Beta = 0x62,                            //b
        ReleaseCandiate = 0x72 | (0x63 << 8),   //rc
        LiveRelease = 0x6C | (0x72 << 8)        //lr
    }

    /// <summary>
    /// Globals for OTA.
    /// </summary>
    public static class Globals
    {
        public const Int32 Build = 5;
        public const ReleasePhase BuildPhase = ReleasePhase.LiveRelease;

        public const Int32 TerrariaRelease = 146;
        public const String TerrariaVersion = "1.3.0.7";

        private const String WorldDirectory = "Worlds";
        private const String PluginDirectory = "Plugins";
        private const String DataDirectory = "Data";
        private const String LibrariesDirectory = "Libraries";
        private const String CharacterData = "Characters";
        //private const String BackupDirectory = "BackupDirectory";

        public static volatile bool Exit = false;

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
        public static string WorldPath
        {
            get
            { return Path.Combine(SavePath, WorldDirectory); }
        }

        //public static string WorldBackupPath
        //{
        //    get
        //    { return Path.Combine(SavePath, WorldDirectory, BackupDirectory); }
        //}

        /// <summary>
        /// Gets the plugin folder.
        /// </summary>
        /// <value>The plugin path.</value>
        public static string PluginPath
        {
            get
            { return Path.Combine(SavePath, PluginDirectory); }
        }

        /// <summary>
        /// Gets the libraries folder.
        /// </summary>
        /// <value>The libraries path.</value>
        public static string LibrariesPath
        {
            get
            { return Path.Combine(SavePath, LibrariesDirectory); }
        }

        /// <summary>
        /// Gets the data folder.
        /// </summary>
        /// <value>The data path.</value>
        public static string DataPath
        {
            get
            { return Path.Combine(SavePath, DataDirectory); }
        }

        /// <summary>
        /// Gets the character data folder.
        /// </summary>
        /// <value>The character data path.</value>
        public static string CharacterDataPath
        {
            get
            { return Path.Combine(SavePath, DataDirectory, CharacterData); }
        }

//        public static readonly bool IsMono = Type.GetType("Mono.Runtime") != null;

        /// <summary>
        /// Creates default required folders
        /// </summary>
        public static void Touch()
        {
            if (!Directory.Exists(SavePath)) Directory.CreateDirectory(SavePath);
//            if (!Directory.Exists(WorldPath)) Directory.CreateDirectory(WorldPath);
            //if (!Directory.Exists(WorldBackupPath)) Directory.CreateDirectory(WorldBackupPath);
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
