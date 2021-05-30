using System;
using ImGuiNET;
using Microsoft.Xna.Framework;
using RuntimeExample.Client;
using Num = System.Numerics;

namespace OTAPI.Client.Host
{
    public class HostGame : Terraria.Main
    {
        private ImGuiRenderer _imGuiRenderer;

        public override void Initialize()
        {
            base.Initialize();
            Console.WriteLine("wrapped  tm");
            _imGuiRenderer = new ImGuiRenderer(this);
            _imGuiRenderer.RebuildFontAtlas();
        }

        private static bool show_test_window = false;
        static Num.Vector3 clear_color = new Num.Vector3(114f / 255f, 144f / 255f, 154f / 255f);

        public event EventHandler ImGuiDraw;

        //byte[] data = new byte[255];

        public override void Draw(GameTime gameTime)
        {
            //Console.WriteLine("Main_Draw");
            this.GraphicsDevice.Clear(new Color(clear_color.X, clear_color.Y, clear_color.Z));

            base.Draw(gameTime);

            //// Call BeforeLayout first to set things up
            _imGuiRenderer.BeforeLayout(gameTime);

            ImGuiDraw?.Invoke(this, EventArgs.Empty);

            //////ImGui.ShowDemoWindow();
            //ImGui.Text("Hello, world!");
            //if (ImGui.Button("Test Window")) show_test_window = !show_test_window;



            //ImGui.InputText("Test", data, (uint)data.Length);

            //// 3. Show the ImGui test window. Most of the sample code is in ImGui.ShowTestWindow()
            //if (show_test_window)
            //{
            //    ImGui.SetNextWindowPos(new Num.Vector2(650, 20), ImGuiCond.FirstUseEver);
            //    ImGui.ShowDemoWindow(ref show_test_window);
            //}

            ////// Draw our UI
            //ImGuiLayout();

            //// Call AfterLayout now to finish up and draw all the things
            _imGuiRenderer.AfterLayout();
        }
    }
}
