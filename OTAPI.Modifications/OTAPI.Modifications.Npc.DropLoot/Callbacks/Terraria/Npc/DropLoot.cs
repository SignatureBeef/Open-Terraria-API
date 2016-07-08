namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Npc
    {
        internal static bool DropLootBegin
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
            var res = Hooks.Npc.PreDropLoot?.Invoke
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
            );
            if (res.HasValue) return res.Value == HookResult.Continue;
            return true;
        }

        internal static void DropLootEnd
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
        ) => Hooks.Npc.PostDropLoot?.Invoke
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
