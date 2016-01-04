#if CLIENT
using System;

namespace OTA.Plugin
{
    public static partial class HookArgs
    {
        public struct GUIChatBoxOpen
        {
            public bool IsEnterDown { get; set; }

            public bool IsNetClient { get; set; }

            public bool IsLeftAltDown { get; set; }

            public bool IsRightAltDown { get; set; }

            /// <summary>
            /// Gets a value indicating whether this the chatbox can be opened to chat with other players.
            /// </summary>
            /// <value><c>true</c> if current state; otherwise, <c>false</c>.</value>
            public bool CanChat
            {
                get
                {
                    return IsEnterDown
                    && IsNetClient
                    && !IsLeftAltDown
                    && !IsRightAltDown
                    && Terraria.Main.hasFocus;
                }
            }

            /// <summary>
            /// Gets a value indicating whether the chatbox can be opened, regardless of game modes
            /// </summary>
            /// <value><c>true</c> if openable; otherwise, <c>false</c>.</value>
            public bool Openable
            {
                get
                {
                    return IsEnterDown
                    && !IsLeftAltDown
                    && !IsRightAltDown
                    && Terraria.Main.hasFocus;
                }
            }
        }
    }

    public static partial class HookPoints
    {
        public static readonly HookPoint<HookArgs.GUIChatBoxOpen> GUIChatBoxOpen = new HookPoint<HookArgs.GUIChatBoxOpen>();
    }
}
#endif