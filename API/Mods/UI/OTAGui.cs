#if CLIENT
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;
using Terraria.UI;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;

namespace OTA.Mod.UI
{
    public enum TerrariaMenu : int
    {
        MainMenu = 0,
        Settings = 18,
        Multiplayer = 12,

    }

    public delegate void MenuButtonClick(MenuButton button);

    public class MenuButton
    {
        public float Scale { get; set; } = 1f;

        public string Text { get; set; }

        public event MenuButtonClick Click;

        public MenuButton(string text)
        {
            this.Text = text;
        }

        public MenuButton(string text, MenuButtonClick click)
        {
            this.Text = text;
            this.Click += click;
        }

        internal void InvokeClick()
        {
            Click.Invoke(this);
        }
    }

    public abstract class OTAGui : INativeMod
    {
        internal static readonly Dictionary <TerrariaMenu, List<MenuButton>> Menus = new Dictionary<TerrariaMenu, List<MenuButton>>();

        #region Instance

        internal void Initialise()
        {
            this.OnInitialise();
        }

        protected virtual void OnInitialise()
        {
        }

        #endregion

        public MenuButton AddMenuButton(TerrariaMenu menu, MenuButton button)
        {
            List<MenuButton> buttons;
            lock (Menus)
            {
                if (!Menus.TryGetValue(menu, out buttons))
                {
                    Menus.Add(menu, buttons = new List<MenuButton>());
                }
            }

            lock (buttons)
            {
                buttons.Add(button);
            }

            return button;
        }

        internal static void EnumerateButtons(TerrariaMenu menu, Action<MenuButton> callback)
        {
            List<MenuButton> buttons;
            lock (Menus)
            {
                if (!Menus.TryGetValue(menu, out buttons))
                {
                    return;
                }
            }

            lock (buttons)
            {
                foreach (var btn in buttons)
                    callback(btn);
            }
        }
    }

    [NativeMod]
    public class UIMenu : OTAGui
    {
        protected override void OnInitialise()
        {
            this.AddMenuButton(TerrariaMenu.MainMenu, new MenuButton("Plugins", (button) =>
                    {
                        Main.PlaySound(10, -1, -1, 1);
                        Main.MenuUI.SetState(UIMenuDisplay.Instance);
                        Main.menuMode = 888;
                    }));
        }
    }

    public static class OTAStyle
    {
        public static Color MainColour { get; } = new Color(114, 192, 44);
    }

    public class UIMenuDisplay : UIState
    {
        public static readonly UIMenuDisplay Instance = new UIMenuDisplay();

        public override void OnInitialize()
        {
            base.OnInitialize();

            var wrapper = CreateWrapper();

            wrapper.Append(CreatePanel());
            wrapper.Append(CreateButtonBack());

            base.Append(wrapper);
        }

        UIElement CreateWrapper()
        {
            var wrapper = new UIElement();
            wrapper.Width.Set(0f, 0.8f);

            var logoHeight = 200f;
            wrapper.Top.Set(logoHeight, 0f);
            wrapper.Height.Set(-logoHeight, 1f);
            wrapper.HAlign = 0.5f;

            var panel = new UIPanel();
            panel.Width.Set(0f, 1f);
            panel.Height.Set(0f, 0.8f);
            panel.BackgroundColor = OTAStyle.MainColour * 0.8f;
            wrapper.Append(panel);

            return wrapper;
        }

        UIPanel CreatePanel()
        {
            var panel = new UIPanel();
            panel.Width.Set(0f, 1f);
            panel.Height.Set(0f, 0.8f);
            panel.BackgroundColor = OTAStyle.MainColour * 0.8f;

            return panel;
        }

        UITextPanel CreateButtonBack()
        {
            var button = new UITextPanel("Back", 1f, false);
            button.Width.Set(0f, 0.4f);
            button.Height.Set(0f, 0.1f);
            button.VAlign = 1f;
            button.Top.Set(0f, -0.05f);

            button.OnClick += new UIElement.MouseEvent(OnBackClick);

            return button;
        }

        void OnBackClick(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.PlaySound(11, Style: 1);
            Main.menuMode = 0;
        }
    }
}
#endif