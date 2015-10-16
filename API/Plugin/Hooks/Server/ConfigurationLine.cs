#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct ConfigurationLine
        {
            public string Key { get; set; }

            public string Value { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.ConfigurationLine> ConfigurationLine = new HookPoint<HookArgs.ConfigurationLine>();
    }
}
#endif