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
using System.Diagnostics;

using ReLogic.Localization.IME;

using MonoMod.RuntimeDetour;

class HostGamePlugin
{
    static Hook LazyHook<TType>(string method, Delegate callback, Type[] types = null)
    {
        var func = typeof(TType).GetMethod(method, types);
        if (func != null) return new Hook(func, callback);
        return null;
    }

    static DialogResult MessageBox_Show(string text, string caption)
    {
        Console.WriteLine($"[MSG {caption}] {text}");
        OTAPI.Client.Launcher.MessageBox.Show(text, caption);
        return DialogResult.Ignore;
    }

    static DialogResult MessageBox_Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        => MessageBox_Show(text, caption);

    [ModFramework.Modification(ModFramework.ModType.Runtime, "Patching windows code to run FNA")]
    static void PatchClient()
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        OTAPI.Hooks.Main.Create = () =>
        {
            return new HostGame();
        };

        // .net5 needs UseShellExecute
        On.Terraria.Utils.OpenFolder += (orig, folder) =>
        {
            using var process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = folder;
            process.Start();
        };

        LazyHook<MessageBox>("Show", new Func<string, string, DialogResult>(MessageBox_Show), new[] { typeof(string), typeof(string) });
        LazyHook<MessageBox>("Show", new Func<string, string, MessageBoxButtons, MessageBoxIcon, DialogResult>(MessageBox_Show), new[] { typeof(string), typeof(string), typeof(MessageBoxButtons), typeof(MessageBoxIcon) });


#if Platform_WINDOWS
        Console.WriteLine("Applying windows hooks");
        // FNA + lack of System.Windows.Forms fixes
        On.ReLogic.OS.Windows.WindowService.SetUnicodeTitle += WindowService_SetUnicodeTitle;
        On.System.Windows.Forms.Control.FromHandle += Control_FromHandle;
        On.ReLogic.OS.Windows.WindowsPlatform.InitializeClientServices += WindowsPlatform_InitializeClientServices;
        On.Terraria.Graphics.WindowStateController.TryMovingToScreen += WindowStateController_TryMovingToScreen;
        On.Terraria.Main.ApplyBorderlessResolution += Main_ApplyBorderlessResolution;
        On.Terraria.Main.SetDisplayModeAsBorderless += Main_SetDisplayModeAsBorderless;
#endif
    }


#if Platform_WINDOWS
    static void Main_ApplyBorderlessResolution(On.Terraria.Main.orig_ApplyBorderlessResolution orig, Form form) { /*nop*/ }

    static void WindowStateController_TryMovingToScreen(On.Terraria.Graphics.WindowStateController.orig_TryMovingToScreen orig, Terraria.Graphics.WindowStateController self, string screenDeviceName) { /*nop*/ }

    static void WindowsPlatform_InitializeClientServices(On.ReLogic.OS.Windows.WindowsPlatform.orig_InitializeClientServices orig, ReLogic.OS.Windows.WindowsPlatform self, IntPtr windowHandle)
    {
        // Windows does not do this in it's binary
        var ime = new FnaIme();
        TextInputEXT.TextInput += ime.OnCharCallback;
        self.RegisterService<IImeService>(ime);
    }

    static void Main_SetDisplayModeAsBorderless(On.Terraria.Main.orig_SetDisplayModeAsBorderless orig, ref int width, ref int height, Form form) { /*nop*/ }

    static Form form;
    static Control Control_FromHandle(On.System.Windows.Forms.Control.orig_FromHandle orig, IntPtr handle)
    {
        // @TODO review handles passed and see if we need a dictionary to map accordingly instead of assuming all need the main form
        if (form == null)
        {
            form = new Form();
            form.ClientSize = new System.Drawing.Size(Terraria.Main.screenWidth, Terraria.Main.screenHeight);
        }
        return form;
    }

    static void WindowService_SetUnicodeTitle(On.ReLogic.OS.Windows.WindowService.orig_SetUnicodeTitle orig, ReLogic.OS.Windows.WindowService self, Microsoft.Xna.Framework.GameWindow window, string title)
    {
        window.Title = title;
    }
#endif

    static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        System.IO.File.AppendAllText("errors-hostgame.txt", e.ExceptionObject.ToString());
    }
}