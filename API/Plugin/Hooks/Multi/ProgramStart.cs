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

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.ProgramStart> ProgramStart = new HookPoint<HookArgs.ProgramStart>();
    }
}
#endif