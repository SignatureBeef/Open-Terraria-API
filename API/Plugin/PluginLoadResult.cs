using System;

namespace OTA.Plugin
{
    /// <summary>
    /// Plugin load result.
    /// </summary>
    public enum PluginLoadResult : int
    {
        Loaded,
        Failed,
        Scheduled,

        InitialiseFailed,
        EnableFailed
    }
}

