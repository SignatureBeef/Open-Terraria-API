#if SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// AddBan hook data
        /// </summary>
        public struct AddBan
        {
            /// <summary>
            /// Calling method position
            /// </summary>
            public MethodState State { get; set; }

            /// <summary>
            /// The slot of the user to ban
            /// </summary>
            public int Slot { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Triggered when Terraria.Netplay.AddBan is called
        /// </summary>
        /// <description>
        /// Set the HookContext to anything but DEFAULT to prevent the ban.
        /// </description>
        public static readonly HookPoint<HookArgs.AddBan> AddBan = new HookPoint<HookArgs.AddBan>();
    }
}
#endif