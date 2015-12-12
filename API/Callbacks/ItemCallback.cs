using Microsoft.Xna.Framework;
using OTA.Callbacks;
using System;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.UI;
using OTA.Plugin;

namespace OTA.Callbacks
{
    public static class ItemCallback
    {
        public static bool OnSetDefaultsBegin(Terraria.Item item, int type = 0, bool noMatCheck = false)
        {
            var ctx = HookContext.Empty;
            var args = new HookArgs.ItemSetDefaultsByType()
            {
                State = MethodState.Begin,
                Type = type,
                NoMatCheck = noMatCheck,

                Item = item
            };

            HookPoints.ItemSetDefaultsByType.Invoke(ref ctx, ref args);

            return ctx.Result == HookResult.DEFAULT;
        }

        public static void OnSetDefaultsEnd(Terraria.Item item, int type = 0, bool noMatCheck = false)
        {
            var ctx = HookContext.Empty;
            var args = new HookArgs.ItemSetDefaultsByType()
            {
                State = MethodState.End,
                Type = type,
                NoMatCheck = noMatCheck,

                Item = item
            };

            HookPoints.ItemSetDefaultsByType.Invoke(ref ctx, ref args);
        }

        public static bool OnSetDefaultsBegin(Terraria.Item item, string itemName)
        {
            var ctx = HookContext.Empty;
            var args = new HookArgs.ItemSetDefaultsByName()
            {
                State = MethodState.Begin,
                Name = itemName,

                Item = item
            };

            HookPoints.ItemSetDefaultsByName.Invoke(ref ctx, ref args);

            return ctx.Result == HookResult.DEFAULT;
        }

        public static void OnSetDefaultsEnd(Terraria.Item item, string itemName)
        {
            var ctx = HookContext.Empty;
            var args = new HookArgs.ItemSetDefaultsByName()
            {
                State = MethodState.End,
                Name = itemName,

                Item = item
            };

            HookPoints.ItemSetDefaultsByName.Invoke(ref ctx, ref args);
        }

        public static bool OnSetDefaultsBegin(Terraria.Item item, int type)
        {
            var ctx = HookContext.Empty;
            var args = new HookArgs.ItemSetDefaultsByType()
            {
                State = MethodState.Begin,
                Type = type,

                Item = item
            };

            HookPoints.ItemSetDefaultsByType.Invoke(ref ctx, ref args);

            return ctx.Result == HookResult.DEFAULT;
        }

        public static void OnSetDefaultsEnd(Terraria.Item item, int type)
        {
            var ctx = HookContext.Empty;
            var args = new HookArgs.ItemSetDefaultsByType()
            {
                State = MethodState.End,
                Type = type,

                Item = item
            };

            HookPoints.ItemSetDefaultsByType.Invoke(ref ctx, ref args);
        }

        public static bool OnNetDefaultsBegin(Terraria.Item item, int type)
        {
            var ctx = HookContext.Empty;
            var args = new HookArgs.ItemNetDefaults()
            {
                State = MethodState.Begin,
                Type = type,

                Item = item
            };

            HookPoints.ItemNetDefaults.Invoke(ref ctx, ref args);

            return ctx.Result == HookResult.DEFAULT;
        }

        public static void OnNetDefaultsEnd(Terraria.Item item, int type)
        {
            var ctx = HookContext.Empty;
            var args = new HookArgs.ItemNetDefaults()
            {
                State = MethodState.End,
                Type = type,

                Item = item
            };

            HookPoints.ItemNetDefaults.Invoke(ref ctx, ref args);
        }

        #if CLIENT

        public static Terraria.Item OnNewItem(int type)
        {
            var ctx = new HookContext();
            var args = new HookArgs.NewItem()
            {
                Type = type
            };

            HookPoints.NewItem.Invoke(ref ctx, ref args);

            if (ctx.Result == HookResult.RECTIFY && ctx.ResultParam is Terraria.Item) return (Terraria.Item)ctx.ResultParam;

            var item = new Terraria.Item();
            item.SetDefaults(type, false);
            return item;
        }

        #endif
    }
}