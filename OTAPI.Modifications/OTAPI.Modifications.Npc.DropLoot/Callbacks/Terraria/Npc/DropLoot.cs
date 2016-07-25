namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Npc
    {
        internal static bool DropLootBegin
        (
            ref int itemId,
            ref int x,
            ref int y,
            ref int width,
            ref int height,
            ref int type,
            ref int stack,
            ref bool noBroadcast,
            ref int prefix,
            ref bool noGrabDelay,
            ref bool reverseLookup
        )
        {
            var res = Hooks.Npc.PreDropLoot?.Invoke
            (
                ref itemId,
                ref x,
                ref y,
                ref width,
                ref height,
                ref type,
                ref stack,
                ref noBroadcast,
                ref prefix,
                ref noGrabDelay,
                ref reverseLookup
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
