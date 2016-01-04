#if CLIENT
using Terraria;
using Microsoft.Xna.Framework.Input;
using OTA.Plugin;
using System;

namespace OTA.Callbacks
{
    public static class InterfaceCallback
    {
        public static bool CanOpenChat()
        {
            var args = new HookArgs.GUIChatBoxOpen()
            {
                IsEnterDown = Main.keyState.IsKeyDown(Keys.Enter),
                IsNetClient = Main.netMode == 1,
                IsLeftAltDown = Main.keyState.IsKeyDown(Keys.LeftAlt),
                IsRightAltDown = Main.keyState.IsKeyDown(Keys.RightAlt)
            };

            if (!Main.chatMode && args.Openable)
            {
                var ctx = new HookContext();

                HookPoints.GUIChatBoxOpen.Invoke(ref ctx, ref args);

                if (ctx.Result == HookResult.RECTIFY)
                {
                    return (bool)ctx.ResultParam;
                }
            }

            return args.CanChat;
        }

        public static bool OnChatTextSend()
        {
            if (!String.IsNullOrEmpty(Terraria.Main.chatText))
            {
                var ctx = new HookContext();
                var args = new HookArgs.GUIChatBoxSend()
                    {
                        Message = Terraria.Main.chatText
                    };

                HookPoints.GUIChatBoxSend.Invoke(ref ctx, ref args);

                if (ctx.Result == HookResult.RECTIFY)
                {
                    Main.chatText = string.Empty;
                    return true; //Will close
                }
                else if (ctx.Result == HookResult.IGNORE)
                    return false;

                return ctx.Result == HookResult.DEFAULT;
            }

            return true;
        }
    }
}
#endif