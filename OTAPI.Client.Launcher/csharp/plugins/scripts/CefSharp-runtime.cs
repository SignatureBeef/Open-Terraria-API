using CefSharp;
using CefSharp.OffScreen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using ImGuiNET;

ChromiumWebBrowser browser;
Texture2D currentFrame = null;

void OnInit(On.Terraria.Main.orig_Initialize orig, Terraria.Main self)
{
    orig(self);
    OnStart();
}
// void OnDraw(On.Terraria.Main.orig_Draw orig, Terraria.Main self, Microsoft.Xna.Framework.GameTime gameTime)
// {
//     orig(self, gameTime);

//     if (currentFrame is not null)
//     {
//         Terraria.Main.spriteBatch.Begin();

//         Terraria.Main.spriteBatch.Draw(currentFrame, new Vector2(0, 0), new Rectangle(0, 0, Terraria.Main.screenWidth / 2, Terraria.Main.screenHeight / 2), Color.White * 0.9f);

//         Terraria.Main.spriteBatch.End();
//     }
// }

bool active = true;
bool enabled = false;
void OnGUI(On.Terraria.Main.orig_OnExtGUI orig)
{
    orig();

    if (currentFrame is not null)
    {
        Terraria.Main.spriteBatch.Draw(currentFrame, new Vector2(0, 0), new Rectangle(0, 0, Terraria.Main.screenWidth / 2, Terraria.Main.screenHeight / 2), Color.White * 0.9f);
    }

    if (active)
    {
        ImGui.Begin("Cef Browser", ref active);
        ImGui.Checkbox("Enable browser", ref enabled);
        ImGui.End();

        if (!active || !enabled)
        {
            browser?.Dispose();
            browser = null;
            currentFrame = null;
        }
        if (enabled && browser is null)
        {
            if (ready)
            {
                browser = new ChromiumWebBrowser("https://google.com.au");
                browser.Paint += Browser_Paint;
            }
        }
    }
}

bool ready = false;
void OnStart()
{
    System.Console.WriteLine("Starting CefSharp");
    ready = true;
}

void Browser_Paint(object? sender, OnPaintEventArgs e)
{
    if (e.DirtyRect.Width == 0 || e.DirtyRect.Height == 0) { return; }

    browser.Size = new System.Drawing.Size(Terraria.Main.screenWidth / 2, Terraria.Main.screenHeight / 2);
    var bmp = browser.ScreenshotOrNull();
    if (bmp != null)
    {
        using MemoryStream stream = new MemoryStream();
        bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        currentFrame = Texture2D.FromStream(Terraria.Main.graphics.GraphicsDevice, stream);
    }
    else currentFrame = null;
}

void OnStop(On.Terraria.Program.orig_ShutdownOTAPI orig)
{
    Cef.Shutdown();
    orig();
}

if (!Cef.IsInitialized)
{
#if ANYCPU
    //Only required for PlatformTarget of AnyCPU
    CefRuntime.SubscribeAnyCpuAssemblyResolver();
#endif
    var settings = new CefSettings()
    {
        //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
        CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache")
    };

    //Example of setting a command line argument
    //Enables WebRTC
    // - CEF Doesn't currently support permissions on a per browser basis see https://bitbucket.org/chromiumembedded/cef/issues/2582/allow-run-time-handling-of-media-access
    // - CEF Doesn't currently support displaying a UI for media access permissions
    //
    //NOTE: WebRTC Device Id's aren't persisted as they are in Chrome see https://bitbucket.org/chromiumembedded/cef/issues/2064/persist-webrtc-deviceids-across-restart
    settings.CefCommandLineArgs.Add("enable-media-stream");
    //https://peter.sh/experiments/chromium-command-line-switches/#use-fake-ui-for-media-stream
    settings.CefCommandLineArgs.Add("use-fake-ui-for-media-stream");
    //For screen sharing add (see https://bitbucket.org/chromiumembedded/cef/issues/2582/allow-run-time-handling-of-media-access#comment-58677180)
    settings.CefCommandLineArgs.Add("enable-usermedia-screen-capturing");

    //Perform dependency check to make sure all relevant resources are in our output directory.
    Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
}

On.Terraria.Program.ShutdownOTAPI += OnStop;
On.Terraria.Main.Initialize += OnInit;
// On.Terraria.Main.Draw += OnDraw;
On.Terraria.Main.OnExtGUI += OnGUI;
if (Terraria.Main.instance is not null)
    OnStart();

Dispose = () =>
{
    On.Terraria.Main.OnExtGUI -= OnGUI;
    On.Terraria.Main.Initialize -= OnInit;
    // On.Terraria.Main.Draw -= OnDraw;
    On.Terraria.Program.ShutdownOTAPI -= OnStop;
    browser?.Dispose();
    browser = null;
    currentFrame = null;
};