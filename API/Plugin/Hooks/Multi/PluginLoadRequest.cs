#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct PluginLoadRequest
        {
            public string Path { get; set; }

            public BasePlugin LoadedPlugin { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.PluginLoadRequest> PluginLoadRequest = new HookPoint<HookArgs.PluginLoadRequest>();
    }
}
#endif