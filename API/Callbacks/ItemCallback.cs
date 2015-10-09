using System;


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
        public static void OnSetDefaultsBegin(Terraria.Item item, int type = 0, bool noMatCheck = false)
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

        public static void OnSetDefaultsBegin(Terraria.Item item, string itemName)
        {
            var ctx = HookContext.Empty;
            var args = new HookArgs.ItemSetDefaultsByName()
            {
                State = MethodState.Begin,
                Name = itemName,

                Item = item
            };

            HookPoints.ItemSetDefaultsByName.Invoke(ref ctx, ref args);
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

        public static void OnSetDefaultsBegin(Terraria.Item item, int type)
        {
            var ctx = HookContext.Empty;
            var args = new HookArgs.ItemSetDefaultsByType()
            {
                State = MethodState.Begin,
                Type = type,

                Item = item
            };

            HookPoints.ItemSetDefaultsByType.Invoke(ref ctx, ref args);
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

        public static void OnNetDefaultsBegin(Terraria.Item item, int type)
        {
            var ctx = HookContext.Empty;
            var args = new HookArgs.ItemNetDefaults()
            {
                State = MethodState.Begin,
                Type = type,

                Item = item
            };

            HookPoints.ItemNetDefaults.Invoke(ref ctx, ref args);
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
    }
}