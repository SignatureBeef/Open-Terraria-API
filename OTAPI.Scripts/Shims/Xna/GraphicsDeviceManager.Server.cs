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
#pragma warning disable CS0436 // Type conflicts with imported type

using Microsoft.Xna.Framework.Graphics;
using System;

namespace Microsoft.Xna.Framework
{
    public class GraphicsDeviceManager
    {
        public GraphicsDeviceManager(Game game) { }

        public bool IsFullScreen { get; set; }
        public int PreferredBackBufferWidth { get; set; }
        public int PreferredBackBufferHeight { get; set; }
        public bool SynchronizeWithVerticalRetrace { get; set; }

        public void ToggleFullScreen() { }
        public void ApplyChanges() { }

        public GraphicsDevice GraphicsDevice { get; set; }
        public GraphicsProfile GraphicsProfile { get; set; }

        public event EventHandler<EventArgs> DeviceCreated;

        public event EventHandler<EventArgs> DeviceResetting;

        public event EventHandler<EventArgs> DeviceReset;

        public event EventHandler<EventArgs> DeviceDisposing;

        public event EventHandler<PreparingDeviceSettingsEventArgs> PreparingDeviceSettings;

        public event EventHandler<EventArgs> Disposed;

        protected virtual bool CanResetDevice(GraphicsDeviceInformation newDeviceInfo) => false;
    }

    public class GraphicsDeviceInformation
    {
        public GraphicsAdapter Adapter { get; set; }

        public GraphicsProfile GraphicsProfile { get; set; }

        public PresentationParameters PresentationParameters { get; set; }

        public override bool Equals(object obj) => false;

        public override int GetHashCode() => 0;

        public GraphicsDeviceInformation Clone() => null;
    }

    public class PreparingDeviceSettingsEventArgs : EventArgs
    {
        public GraphicsDeviceInformation GraphicsDeviceInformation { get; }

        public PreparingDeviceSettingsEventArgs(GraphicsDeviceInformation graphicsDeviceInformation) { }
    }
}