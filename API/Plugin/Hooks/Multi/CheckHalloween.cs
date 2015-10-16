#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct CheckHalloween
        {
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.CheckHalloween> CheckHalloween = new HookPoint<HookArgs.CheckHalloween>();
    }
}
#endif