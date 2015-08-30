namespace OTA
{
    /// <summary>
    /// OTA server states for hooks
    /// </summary>
    public enum ServerState
    {
        WorldGenerating,
        WorldGenerated,
        Initialising,
        Starting,
        WorldLoading,
        WorldLoaded,
        Restarting,
        Stopping
    }
}