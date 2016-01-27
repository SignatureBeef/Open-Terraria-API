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

        //InterfaceCallback.OnDrawMenu (ref color, ref color2, ref color3, ref pressedKeys, ref pressedKeys2, ref flag, ref flag2,
        //ref flag3, ref flag4, ref flag5, ref flag6, ref array, ref array2, ref array3, ref array8, ref b, ref array6, ref i,
        //ref num, ref num2, ref num3, ref num4, ref num5, ref num6, ref num7, ref num8, ref j, ref k, ref num9, ref l, ref m,
        //ref num10, ref n, ref num11, ref num12, ref num14, ref num15, ref num16, ref num17, ref num18, ref num19, ref num20,
        //ref num21, ref num22, ref num23, ref num24, ref num25, ref num26, ref num27, ref num28, ref num29, ref num30, ref num31,
        //ref num32, ref num33, ref num34, ref array4, ref array5, ref num13, ref array7, ref text, ref serverPassword, ref serverPassword2,
        //ref text2, ref text3, ref name, ref a, ref a2, ref a3, ref a4, ref a5, ref array9, ref array10, ref array11, ref array12);
        //Injected before the last 888
        public static void OnDrawMenu(
            ref Microsoft.Xna.Framework.Color prm0,
            ref Microsoft.Xna.Framework.Color prm1,
            ref Microsoft.Xna.Framework.Color prm2,
            ref Microsoft.Xna.Framework.Input.Keys[] prm3,
            ref Microsoft.Xna.Framework.Input.Keys[] prm4,
            ref System.Boolean prm5,
            ref System.Boolean prm6,
            ref System.Boolean prm7,
            ref System.Boolean prm8,
            ref System.Boolean prm9,
            ref System.Boolean prm10,
            ref System.Boolean[] prm11,
            ref System.Boolean[] prm12,
            ref System.Boolean[] prm13,
            ref System.Boolean[] prm14,
            ref System.Byte prm15,
            ref System.Byte[] prm16,
            ref System.Int32 prm17,
            ref System.Int32 buttonOffsetY,
            ref System.Int32 prm19,
            ref System.Int32 buttonSpacing,
            ref System.Int32 buttonCount,
            ref System.Int32 prm22,
            ref System.Int32 prm23,
            ref System.Int32 prm24,
            ref System.Int32 prm25,
            ref System.Int32 prm26,
            ref System.Int32 prm27,
            ref System.Int32 prm28,
            ref System.Int32 prm29,
            ref System.Int32 prm30,
            ref System.Int32 prm31,
            ref System.Int32 prm32,
            ref System.Int32 targetIndex,
            ref System.Int32 prm34,
            ref System.Int32 prm35,
            ref System.Int32 prm36,
            ref System.Int32 prm37,
            ref System.Int32 prm38,
            ref System.Int32 prm39,
            ref System.Int32 prm40,
            ref System.Int32 prm41,
            ref System.Int32 prm42,
            ref System.Int32 prm43,
            ref System.Int32 prm44,
            ref System.Int32 prm45,
            ref System.Int32 prm46,
            ref System.Int32 prm47,
            ref System.Int32 prm48,
            ref System.Int32 prm49,
            ref System.Int32 prm50,
            ref System.Int32 prm51,
            ref System.Int32 prm52,
            ref System.Int32 prm53,
            ref System.Int32 prm54,
            ref System.Int32 prm55,
            ref System.Int32[] prm56,
            ref System.Int32[] array5,
            ref System.Single prm58,
            ref System.Single[] buttonScales,
            ref System.String prm60,
            ref System.String prm61,
            ref System.String prm62,
            ref System.String prm63,
            ref System.String prm64,
            ref System.String prm65,
            ref System.String prm66,
            ref System.String prm67,
            ref System.String prm68,
            ref System.String prm69,
            ref System.String prm70,
            ref System.String[] buttonNames,
            ref System.String[] prm72,
            ref System.String[] prm73,
            ref System.String[] prm74)
        {
            var lButtonScales = buttonScales;
            var lButtonNames = buttonNames;
            var lButtonCount = buttonCount;
            var lTargetIndex = targetIndex;
            OTA.Mod.UI.OTAGui.EnumerateButtons((OTA.Mod.UI.TerrariaMenu)Main.menuMode, (button) =>
                {
                    lButtonScales[lButtonCount] = button.Scale;
                    lButtonNames[lButtonCount] = button.Text;

                    if(Main.instance.selectedMenu == lTargetIndex)
                    {
                        button.InvokeClick();
                    }

                    lButtonCount++;
                });
            buttonScales = lButtonScales;
            buttonNames = lButtonNames;
            buttonCount = lButtonCount;
        }
    }
}
#endif