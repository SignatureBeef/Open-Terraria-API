/*
Copyright (C) 2020 DeathCradle

This file is part of Open Terraria API v3 (OTAPI)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/

using ModFramework;
using System;
using System.Windows.Forms;
using System.IO;
using Microsoft.Xna.Framework;
using Num = System.Numerics;

using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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