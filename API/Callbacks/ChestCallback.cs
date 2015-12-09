using System;
using OTA.Plugin;

namespace OTA.Callbacks
{
    public static class ChestCallback
    {
        public static bool OnSetupShopBegin(Terraria.Chest chest, int type)
        {
            var ctx = new HookContext();
            var args = new HookArgs.ChestSetupShop()
            {
                Chest = chest,
                Type = type
            };

            HookPoints.ChestSetupShop.Invoke(ref ctx, ref args);

            return ctx.Result == HookResult.DEFAULT;
        }

        public static void OnSetupShopEnd(Terraria.Chest chest, int type)
        {
            var ctx = HookContext.Empty;
            var args = new HookArgs.ChestSetupShop()
            {
                Chest = chest,
                Type = type
            };

            HookPoints.ChestSetupShop.Invoke(ref ctx, ref args);
        }
    }
}

