#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Start command processing data.
        /// </summary>
        public struct StartCommandProcessing
        {
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Occurs when vanilla/OTA wants to start the command thread.
        /// </summary>
        /// <description>
        /// If the HookContext.Result is set to anything other than DEFAULT then vanilla/OTA won't start it's thread.
        /// It's reccommended if you want to roll your own that you set HookResult.IGNORE.
        /// </description>
        public static readonly HookPoint<HookArgs.StartCommandProcessing> StartCommandProcessing = new HookPoint<HookArgs.StartCommandProcessing>();
    }
}
#endif