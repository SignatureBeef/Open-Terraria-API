#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct AddBan
        {
            public string RemoteAddress { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.AddBan> AddBan = new HookPoint<HookArgs.AddBan>();
    }
}
#endif