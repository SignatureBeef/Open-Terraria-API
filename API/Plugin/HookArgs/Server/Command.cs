#if SERVER
using System;
using OTA.Command;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct Command
        {
            public string Prefix { get; internal set; }
            public ArgumentList Arguments { get; set; }
            public string ArgumentString { get; set; }
        }
    }
}
#endif