using System;

namespace OTA.Plugin
{
    public delegate void HookAction<T>(ref HookContext context,ref T argument);
}

