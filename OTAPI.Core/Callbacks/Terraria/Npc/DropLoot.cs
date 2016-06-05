namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Npc
    {
        public static bool DropLootBegin
        (
            ref int itemId,
            int x,
            int y,
            int width,
            int height,
            int type,
            int stack,
            bool noBroadcast,
            int prefix,
            bool noGrabDelay,
            bool reverseLookup
        )
        {
            if (Hooks.Npc.PreDropLoot != null)
                return Hooks.Npc.PreDropLoot
                (
                    ref itemId,
                    x,
                    y,
                    width,
                    height,
                    type,
                    stack,
                    noBroadcast,
                    prefix,
                    noGrabDelay,
                    reverseLookup
                ) == HookResult.Continue;
            return true;
        }

        public static void DropLootEnd
        (
            int x,
            int y,
            int width,
            int height,
            int type,
            int stack,
            bool noBroadcast,
            int prefix,
            bool noGrabDelay,
            bool reverseLookup
        )
        {
            if (Hooks.Npc.PostDropLoot != null)
                Hooks.Npc.PostDropLoot
                (
                    x,
                    y,
                    width,
                    height,
                    type,
                    stack,
                    noBroadcast,
                    prefix,
                    noGrabDelay,
                    reverseLookup
                );
        }
    }
}
