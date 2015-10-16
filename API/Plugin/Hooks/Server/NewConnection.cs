#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct NewConnection
        {
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.NewConnection> NewConnection = new HookPoint<HookArgs.NewConnection>();
    }
}
#endif