namespace OTAPI
{
    /// <summary>
    /// Defines a result from a hook to be actioned by OTAPI
    /// </summary>
    public enum HookResult : int
    {
        /// <summary>
        /// Typically used to continue on to vanilla code
        /// </summary>
        Continue,

        /// <summary>
        /// Typically used to stop executing vanilla code
        /// </summary>
        Cancel
    }
}
