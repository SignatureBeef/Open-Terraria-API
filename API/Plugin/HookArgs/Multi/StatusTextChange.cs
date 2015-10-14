#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct StatusTextChange
        {
            public static readonly StatusTextChange Empty = new StatusTextChange();
        }
    }
}
#endif