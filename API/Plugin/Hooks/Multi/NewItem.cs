#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Item info
        /// </summary>
        public struct NewItem
        {
            /// <summary>
            /// Type of Item to be created
            /// </summary>
            public int Type { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Occurs when vanilla calls Terraria.Item.NewItem and a new Item is to be created
        /// </summary>
        public static readonly HookPoint<HookArgs.NewItem> NewItem = new HookPoint<HookArgs.NewItem>();
    }
}
#endif