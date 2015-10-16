#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct WorldAutoSave { }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.WorldAutoSave> WorldAutoSave = new HookPoint<HookArgs.WorldAutoSave>();
    }
}
#endif