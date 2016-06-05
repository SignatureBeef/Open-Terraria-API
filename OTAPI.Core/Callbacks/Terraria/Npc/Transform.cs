namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Npc
    {
        internal static void Transform(global::Terraria.NPC npc)
        {
            if (Hooks.Npc.Transform != null)
                Hooks.Npc.Transform(npc);
        }
    }
}
