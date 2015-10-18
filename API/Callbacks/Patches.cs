using System;
using OTA.Command;
using OTA.Plugin;
using OTA.Misc;
using OTA.Logging;

namespace OTA.Callbacks
{
    /// <summary>
    /// Callbacks from vanilla code for miscellaneous patches
    /// </summary>
    public static class Patches
    {
        /// <summary>
        /// Used in vanilla code where there was fixed windows paths
        /// </summary>
        /// <returns>The current directory.</returns>
        public static string GetCurrentDirectory()
        {
            return Environment.CurrentDirectory;
        }
    }
}
