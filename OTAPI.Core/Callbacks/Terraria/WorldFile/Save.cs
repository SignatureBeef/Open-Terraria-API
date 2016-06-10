namespace OTAPI.Core.Callbacks.Terraria
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
            if (Hooks.World.IO.PreSaveWorld != null)
                return Hooks.World.IO.PreSaveWorld(ref useCloudSaving, ref resetTime) == HookResult.Continue;
            return true;
        }

        /// <summary>
        /// This method is injected into the end of the saveWorld(bool) method.
        /// </summary>
        internal static void SaveWorldEnd(bool useCloudSaving, bool resetTime)
        {
            if (Hooks.World.IO.PostSaveWorld != null)
                Hooks.World.IO.PostSaveWorld(useCloudSaving, resetTime);
        }
    }
}
