#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct PluginsLoaded
        {
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.PluginsLoaded> PluginsLoaded = new HookPoint<HookArgs.PluginsLoaded>();
    }
}
#endif