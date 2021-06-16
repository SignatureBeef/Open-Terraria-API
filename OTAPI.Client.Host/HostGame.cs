using System;
using System.IO;
using Microsoft.Xna.Framework;
using ModFramework.Modules.ClearScript.Typings;
using Num = System.Numerics;

namespace OTAPI.Client.Host
{
    public class HostGame : Terraria.Main
    {
        private ImGuiRenderer _imGuiRenderer;

        public HostGame()
        {
            bool isHighDPI = Environment.GetEnvironmentVariable("FNA_GRAPHICS_ENABLE_HIGHDPI") == "1";
            if (isHighDPI)
                Console.WriteLine("HiDPI Enabled");
        }

        public override void Initialize()
        {
            base.Initialize();

            Terraria.Main.SkipAssemblyLoad = true;

            _imGuiRenderer = new ImGuiRenderer(this);
            _imGuiRenderer.RebuildFontAtlas();
        }

        static Num.Vector3 clear_color = new Num.Vector3(114f / 255f, 144f / 255f, 154f / 255f);

        public event EventHandler ImGuiDraw;

        public override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(new Color(clear_color.X, clear_color.Y, clear_color.Z));

            base.Draw(gameTime);

            _imGuiRenderer.BeforeLayout(gameTime);
            ImGuiDraw?.Invoke(this, EventArgs.Empty);
            _imGuiRenderer.AfterLayout();
        }
    }
}
