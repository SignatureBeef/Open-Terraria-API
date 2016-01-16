#if CLIENT || SERVER
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        /// <summary>
        /// Window title data
        /// </summary>
        public struct SetWindowTitle
        {
            /// <summary>
            /// The title about to be set
            /// </summary>
            /// <value>The title.</value>
            public string Title { get; set; }
        }
    }

    public static partial class HookPoints
    {
        /// <summary>
        /// Occurs when either Console.Title or Terraria.Main.SetTitle is called from Terraria.
        /// </summary>
        public static readonly HookPoint<HookArgs.SetWindowTitle> SetWindowTitle = new HookPoint<HookArgs.SetWindowTitle>();
    }
}
#endif