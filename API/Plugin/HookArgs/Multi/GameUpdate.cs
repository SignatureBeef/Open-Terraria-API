#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct GameUpdate
        {
            public static readonly GameUpdate Begin = new GameUpdate() { State = MethodState.Begin };
            public static readonly GameUpdate End = new GameUpdate() { State = MethodState.End };

            public MethodState State { get; set; }
        }
    }
}
#endif