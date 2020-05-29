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
using System;

namespace Microsoft.Xna.Framework
{
    [Flags]
    public enum DisplayOrientation
    {
        Default = 0x0,
        LandscapeLeft = 0x1,
        LandscapeRight = 0x2,
        Portrait = 0x4
    }

    public class GameWindow
    {
        public event EventHandler<EventArgs> ScreenDeviceNameChanged;

        public event EventHandler<EventArgs> ClientSizeChanged;

        public event EventHandler<EventArgs> OrientationChanged;

        public string Title
        {
            get
            { return Console.Title; }
            set
            {
                SetTitle(value);
            }
        }

        public static void SetTitle(string title)
        {
        }

        public IntPtr Handle { get; set; }

        public bool AllowUserResizing { get; set; }
        public Rectangle ClientBounds { get; }

        public string ScreenDeviceName { get; }
        public DisplayOrientation CurrentOrientation { get; }

        public void BeginScreenDeviceChange(bool willBeFullScreen) { }

        public void EndScreenDeviceChange(string screenDeviceName, int clientWidth, int clientHeight) { }

        public void EndScreenDeviceChange(string screenDeviceName) { }
    }
}