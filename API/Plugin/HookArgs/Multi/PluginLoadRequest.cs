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
}
#endif