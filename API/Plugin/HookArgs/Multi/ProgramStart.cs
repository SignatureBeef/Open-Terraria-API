#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct ProgramStart
        {
            public string[] Arguments { get; set; }
        }
    }
}
#endif