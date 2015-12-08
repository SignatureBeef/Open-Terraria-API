#if CLIENT
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct NpcLoadTexture
        {
            public int NpcTypeId { get; set; }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.NpcLoadTexture> NpcLoadTexture = new HookPoint<HookArgs.NpcLoadTexture>();
    }
}
#endif