#if CLIENT
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;
using Terraria.UI;
using Terraria;

namespace OTA.Mod.UI
{
    public enum TerrariaMenu : int
    {
        MainMenu = 0,
        Settings = 18,
        Multiplayer = 12,

    }

    public class MenuButton
    {
        public float Scale { get; set; } = 1f;

        public string Text { get; set; }

        public EventHandler Click { get; set; }

        public MenuButton(string text)
        {
            this.Text = text;
        }

        internal void InvokeClick()
        {
            Click.Invoke(this, EventArgs.Empty);
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
            var btn = this.AddMenuButton(TerrariaMenu.MainMenu, new MenuButton("Plugins"));
            btn.Click += (sender, args) =>
            {
                Main.PlaySound(10, -1, -1, 1);
                Main.MenuUI.SetState(UIMenuDisplay.Instance);
                Main.menuMode = 888;
            };
        }
    }

    public class UIMenuDisplay : UIState
    {
        public static readonly UIMenuDisplay Instance = new UIMenuDisplay();

        public override void OnInitialize()
        {
            base.OnInitialize();
            Console.WriteLine("Initialised menu! =)");
        }
    }
}
#endif