namespace OTAPI.Callbacks.Terraria
{
    internal static partial class WorldFile
    {
        internal static bool LoadWorldBegin(ref bool loadFromCloud)
        {
            var res = Hooks.World.IO.PreLoadWorld?.Invoke(ref loadFromCloud);
            if (res.HasValue) return res.Value == HookResult.Continue;
            return true;
        }

        internal static void LoadWorldEnd(bool loadFromCloud) =>
            Hooks.World.IO.PostLoadWorld?.Invoke(loadFromCloud);
    }
}
