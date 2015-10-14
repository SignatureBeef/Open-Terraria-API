#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct ServerUpdate
        {
            public static readonly ServerUpdate Begin = new ServerUpdate() { State = MethodState.Begin };
            public static readonly ServerUpdate End = new ServerUpdate() { State = MethodState.End };

            public MethodState State { get; set; }
        }
    }
}
#endif