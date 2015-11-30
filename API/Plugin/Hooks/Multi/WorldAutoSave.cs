#if SERVER || CLIENT
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// World auto save data.
        /// </summary>
        public struct WorldAutoSave { }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Occurs when vanilla auto saves the world data.
        /// </summary>
        /// <description>Set the HookContext.Result to anything other than DEFAULT to prevent the save.</description>
        public static readonly HookPoint<HookArgs.WorldAutoSave> WorldAutoSave = new HookPoint<HookArgs.WorldAutoSave>();
    }
}
#endif