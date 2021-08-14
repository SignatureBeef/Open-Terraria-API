using CefSharp;
using CefSharp.OffScreen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

ChromiumWebBrowser browser;
Texture2D currentFrame = null;

void OnInit(On.Terraria.Main.orig_Initialize orig, Terraria.Main self)
{
    orig(self);
    OnStart();
}
void OnDraw(On.Terraria.Main.orig_Draw orig, Terraria.Main self, Microsoft.Xna.Framework.GameTime gameTime)
{
    orig(self, gameTime);

    if (currentFrame is not null)
    {
        Terraria.Main.spriteBatch.Begin();

        Terraria.Main.spriteBatch.Draw(currentFrame, new Vector2(0, 0), new Rectangle(0, 0, Terraria.Main.screenWidth / 4, Terraria.Main.screenHeight / 4), Color.White * 0.9f);

        Terraria.Main.spriteBatch.End();
    }
}

void OnStart()
{
    System.Console.WriteLine("Starting CefSharp");

    browser = new ChromiumWebBrowser("https://google.com.au");

    browser.Paint += Browser_Paint;
}

void Browser_Paint(object? sender, OnPaintEventArgs e)
{
    if (e.DirtyRect.Width == 0 || e.DirtyRect.Height == 0) { return; }

    browser.Size = new System.Drawing.Size(Terraria.Main.screenWidth / 4, Terraria.Main.screenHeight / 4);
    var bmp = browser.ScreenshotOrNull();
    if (bmp != null)
    {
        using MemoryStream stream = new MemoryStream();
        bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        currentFrame = Texture2D.FromStream(Terraria.Main.graphics.GraphicsDevice, stream);
    }
    else currentFrame = null;
}

On.Terraria.Main.Initialize += OnInit;
On.Terraria.Main.Draw += OnDraw;

if (Terraria.Main.instance is not null)
{
    OnStart();
}

Dispose = () =>
{
    On.Terraria.Main.Initialize -= OnInit;
    On.Terraria.Main.Draw -= OnDraw;
    browser?.Dispose();
    browser = null;
    currentFrame = null;
};