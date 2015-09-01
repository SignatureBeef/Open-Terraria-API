namespace OTA
{
    /// <summary>
    /// OTA server states for hooks
    /// </summary>
    // TODO Ensure all these are called where required
    public enum ServerState
    {
        /// <summary>
        /// When the server is first loading. This is never raised as it's the default state before any plugins are loaded.
        /// </summary>
        PreInitialisation,

        /// <summary>
        /// When the server is generating a world
        /// </summary>
        WorldGenerating,

        /// <summary>
        /// When the server has successfully generated a world
        /// </summary>
        WorldGenerated,

        /// <summary>
        /// When the server itself is initialising the server for connections
        /// </summary>
        Initialising,

        /// <summary>
        /// When the TCP server is starting
        /// </summary>
        Starting,

        /// <summary>
        /// When the server has started loading a world
        /// </summary>
        WorldLoading,

        /// <summary>
        /// When the server has loaded a world
        /// </summary>
        WorldLoaded,

        /// <summary>
        /// When the server has began restarting
        /// </summary>
        Restarting,

        /// <summary>
        /// When the connection server has stopped
        /// </summary>
        Stopping
    }
}