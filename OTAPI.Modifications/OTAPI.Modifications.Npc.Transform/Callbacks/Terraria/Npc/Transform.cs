namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Npc
    {
        internal static void Transform(global::Terraria.NPC npc) => Hooks.Npc.Transform?.Invoke(npc);
    }
}
