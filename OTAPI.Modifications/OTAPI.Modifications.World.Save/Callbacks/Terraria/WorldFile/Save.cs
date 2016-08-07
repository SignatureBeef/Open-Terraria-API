namespace OTAPI.Callbacks.Terraria
{
    internal static partial class WorldFile
    {
        /// <summary>
        /// This method is injected into the start of the saveWorld(bool) method.
        /// The return value will dictate if normal vanilla code should continue to run.
        /// </summary>
        /// <returns>True to continue on to vanilla code, otherwise false</returns>
        internal static bool SaveWorldBegin(ref bool useCloudSaving, ref bool resetTime)
        {
            var res = Hooks.World.IO.PreSaveWorld?.Invoke(ref useCloudSaving, ref resetTime);
            if (res.HasValue) return res.Value == HookResult.Continue;
            return true;
        }

        /// <summary>
        /// This method is injected into the end of the saveWorld(bool) method.
        /// </summary>
        internal static void SaveWorldEnd(bool useCloudSaving, bool resetTime) =>
            Hooks.World.IO.PostSaveWorld?.Invoke(useCloudSaving, resetTime);
    }
}
