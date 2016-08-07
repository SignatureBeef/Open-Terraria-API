namespace OTAPI.Core
{
    public enum HardmodeTileUpdateResult
    {
        /// <summary>
        /// Continue to update the tile
        /// </summary>
        Continue,

        /// <summary>
        /// Cancel updating the tile
        /// </summary>
        Cancel,

        /// <summary>
        /// Continue vanilla code, but don't update the tile
        /// </summary>
        ContinueWithoutUpdate
    }
}
