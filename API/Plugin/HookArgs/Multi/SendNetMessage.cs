#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct SendNetMessage
        {
            public int MsgType { get; set; }
            public int BufferId { get; set; }
            public int RemoteClient { get; set; }
            public int IgnoreClient { get; set; }
            public string Text { get; set; }
            public int Number { get; set; }
            public float Number2 { get; set; }
            public float Number3 { get; set; }
            public float Number4 { get; set; }
            public int Number5 { get; set; }
        }
    }
}
#endif