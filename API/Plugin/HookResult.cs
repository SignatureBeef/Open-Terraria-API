using System;

namespace OTA.Plugin
{
    public enum HookResult
    {
        DEFAULT = 0,
        CONTINUE,
        KICK,
        ASK_PASS,
        IGNORE,
        RECTIFY,
        ERASE
    }
}

